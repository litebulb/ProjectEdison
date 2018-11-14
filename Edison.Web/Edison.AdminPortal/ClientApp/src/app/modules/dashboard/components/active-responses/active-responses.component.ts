import { Component, OnInit, OnDestroy } from '@angular/core'
import {
    Response,
    ResponseState,
} from '../../../../reducers/response/response.model'
import { Store, select } from '@ngrx/store'
import { AppState } from '../../../../reducers'
import {
    activeResponsesSelector,
    activeResponseSelector,
} from '../../../../reducers/response/response.selectors'
import { fadeInOut } from '../../../../core/animations/fadeInOut'
import { SearchListItem } from '../../../../core/models/searchListItem'
import {
    SelectActiveResponse,
    GetResponse,
} from '../../../../reducers/response/response.actions'
import { Subscription } from 'rxjs'
import { SetSelectingActionPlan } from '../../../../reducers/action-plan/action-plan.actions'
import { ActionPlanStatus } from '../../../../reducers/action-plan/action-plan.model'

enum ActiveView {
    Default,
    Deactivate,
    Manage,
    Update,
}

@Component({
    selector: 'app-active-responses',
    templateUrl: './active-responses.component.html',
    styleUrls: [ './active-responses.component.scss' ],
    animations: [ fadeInOut ],
})
export class ActiveResponsesComponent implements OnInit, OnDestroy {
    active = false
    items: SearchListItem[]
    responses: Response[]
    activeResponses: Response[]
    activeResponse: Response
    activeId = null
    activeView = ActiveView.Default
    scrollConfig = { suppressScrollX: false, suppressScrollY: true, useBothWheelAxes: true, scrollIndicators: true }
    loadingFullResponse = false;

    private activeResponsesSub$: Subscription
    private activeResponseSub$: Subscription

    constructor (private store: Store<AppState>) { }

    ngOnInit() {
        this.activeResponsesSub$ = this.store
            .pipe(select(activeResponsesSelector))
            .subscribe(responses => {
                this.activeResponses = responses

                let responsesToShow = responses
                if (this.activeResponse) {
                    const activeResponseResolved = !responsesToShow.some(
                        response => response.responseId === this.activeResponse.responseId
                    )
                    if (activeResponseResolved) {
                        // we want to let this response still show until the active responses window closes
                        responsesToShow = [
                            ...responses,
                            {
                                ...this.activeResponse,
                                responseState: ResponseState.Inactive,
                            },
                        ].sort((a, b) => a.actionPlan.name.localeCompare(b.actionPlan.name))
                    }
                }

                this.refreshResponses(responsesToShow)
            })

        this.activeResponseSub$ = this.store
            .pipe(select(activeResponseSelector))
            .subscribe(({ activeResponse, openManageResponse }) => {
                if (activeResponse && openManageResponse) {
                    this.openResponse(activeResponse)
                }
            })
    }

    ngOnDestroy() {
        this.activeResponsesSub$.unsubscribe()
        this.activeResponseSub$.unsubscribe()
    }

    responseUpdated() {

    }

    refreshResponses(responses: Response[]) {
        this.items = responses.map(r => ({
            id: r.responseId,
            name: r.name,
            icon: r.icon,
            color: r.color,
        }))
        this.responses = responses

        if (this.activeResponse) {
            this.activeResponse = responses.find(
                response => response.responseId === this.activeResponse.responseId
            )
            this.updateCloseActions()
        }
    }

    selectActiveResponse(item: SearchListItem) {
        if (item) {
            this.activeResponse = this.responses.find(r => r.responseId === item.id)
            this.getFullResponse()
            this.updateCloseActions()
        } else {
            this.deactivateActiveResponse()
        }
        this.activeView = ActiveView.Default
    }

    openResponse(response: Response) {
        this.active = true
        this.activeResponse = response
        this.activeId = response.responseId
        this.activeView = ActiveView.Default

        this.getFullResponse()
        this.store.dispatch(new SetSelectingActionPlan({ isSelecting: true }))
    }

    updateCloseActions() {
        if (
            this.activeResponse &&
            this.activeResponse.responseState === ResponseState.Inactive &&
            this.activeResponse.actionPlan &&
            this.activeResponse.actionPlan.closeActions
        ) {
            this.activeResponse = {
                ...this.activeResponse,
                actionPlan: {
                    ...this.activeResponse.actionPlan,
                    closeActions: this.activeResponse.actionPlan.closeActions.map(a => ({
                        ...a,
                        status: ActionPlanStatus.Complete,
                    })),
                },
            }
        }
    }

    getFullResponse() {
        if (!this.activeResponse.actionPlan) {
            this.loadingFullResponse = true;
            this.store.dispatch(new GetResponse({ responseId: this.activeResponse.responseId }));
        } else {
            this.loadingFullResponse = false;
        }
    }

    showDeactivateView() {
        this.activeView = ActiveView.Deactivate
    }

    showUpdateView() {
        this.activeView = ActiveView.Update
    }

    backClicked() {
        this.activeView = ActiveView.Default
    }

    toggleResponses() {
        if (this.active) {
            this.deactivateActiveResponse()
        }
        this.active = !this.active

        if (!this.active) {
            this.refreshResponses(this.activeResponses) // clear out any resolved responses on close
        }

        this.store.dispatch(
            new SetSelectingActionPlan({ isSelecting: this.active })
        )
    }

    deactivateActiveResponse() {
        this.activeId = null
        this.activeResponse = null
        this.store.dispatch(new SelectActiveResponse({ response: null }))
    }
}
