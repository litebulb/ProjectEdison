import { Subscription } from 'rxjs';

import { Component, OnDestroy, OnInit } from '@angular/core';
import { select, Store } from '@ngrx/store';

import { fadeInOut } from '../../../../core/animations/fadeInOut';
import { SearchListItem } from '../../../../core/models/searchListItem';
import { AppState } from '../../../../reducers';
import { SetSelectingActionPlan } from '../../../../reducers/action-plan/action-plan.actions';
import { GetResponse, SelectActiveResponse } from '../../../../reducers/response/response.actions';
import { Response, ResponseState } from '../../../../reducers/response/response.model';
import {
    activeResponseSelector, activeResponsesSelector, responsesSelector
} from '../../../../reducers/response/response.selectors';

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
    active = false;
    items: SearchListItem[];
    responses: Response[];
    activeResponses: Response[];
    activeResponse: Response;
    activeId = null;
    activeView = ActiveView.Default;
    scrollConfig = { suppressScrollX: true, suppressScrollY: false, useBothWheelAxes: true, scrollIndicators: true };
    loadingFullResponse = false;;

    private activeResponsesSub$: Subscription;
    private activeResponseSub$: Subscription;
    private responsesSub$: Subscription;

    constructor (private store: Store<AppState>) { }

    ngOnInit() {
        this.activeResponsesSub$ = this.store
            .pipe(select(activeResponsesSelector))
            .subscribe(responses => {
                this.activeResponses = [ ...responses ]

                let responsesToShow = [ ...responses ]
                if (this.activeResponse) {
                    const activeResponse = responsesToShow.find(
                        response => response.responseId === this.activeResponse.responseId
                    )
                    if (!activeResponse) {
                        // we want to let this response still show until the active responses window closes
                        responsesToShow = [
                            ...responses,
                            {
                                ...this.activeResponse,
                                responseState: ResponseState.Inactive,
                            },
                        ].filter(resp => resp.name).sort((a, b) => a.name.localeCompare(b.name))
                    } else {
                        this.activeResponse = activeResponse;
                        this.activeId = activeResponse.responseId;

                        this.getFullResponse();
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

        this.responsesSub$ = this.store
            .pipe(select(responsesSelector))
            .subscribe(responses => {
                if (this.activeResponse) {
                    this.activeResponse = responses.find(resp => resp.responseId === this.activeResponse.responseId);
                }
            })
    }

    ngOnDestroy() {
        this.activeResponsesSub$.unsubscribe()
        this.activeResponseSub$.unsubscribe()
        this.responsesSub$.unsubscribe();
    }

    responseUpdated() {

    }

    getIconStyle(index: number) {
        if (index < 5) { return null; }

        const style = {
            'margin-right': '10px',
            'margin-top': '6px',
        };

        return style;
    }

    refreshResponses(responses: Response[]) {
        this.items = responses.map(r => ({
            id: r.responseId,
            name: r.name,
            icon: r.icon,
            color: r.color,
        }));
        this.responses = responses;
    }

    selectActiveResponse(item: SearchListItem) {
        if (item) {
            this.activeResponse = { ...this.responses.find(r => r.responseId === item.id) }
            this.getFullResponse()
        } else {
            this.deactivateActiveResponse()
        }
        this.activeView = ActiveView.Default
    }

    openResponse(response: Response) {
        this.active = true
        this.activeResponse = { ...response }
        this.activeId = this.activeResponse.responseId
        this.activeView = ActiveView.Default

        this.getFullResponse()
        this.store.dispatch(new SetSelectingActionPlan({ isSelecting: true }))
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
