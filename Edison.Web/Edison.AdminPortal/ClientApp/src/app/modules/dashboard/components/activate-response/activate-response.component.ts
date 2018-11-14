import {
    Component,
    OnInit,
    OnDestroy,
} from '@angular/core'
import { fadeInOut } from '../../../../core/animations/fadeInOut'
import { Store, select } from '@ngrx/store'
import { AppState } from '../../../../reducers'
import {
    eventsSelector,
    activeEventSelector,
} from '../../../../reducers/event/event.selectors'
import {
    ActionPlan,
    ActionPlanStatus,
} from '../../../../reducers/action-plan/action-plan.model'
import {
    selectedActionPlanSelector,
    actionPlansSelector,
} from '../../../../reducers/action-plan/action-plan.selectors'
import {
    SelectActionPlan,
    GetActionPlans,
    GetActionPlan,
    SetSelectingActionPlan,
    PutActionPlan,
} from '../../../../reducers/action-plan/action-plan.actions'
import { PostNewResponse, ResponseActionTypes, ShowActivateResponse } from '../../../../reducers/response/response.actions'
import { Event } from '../../../../reducers/event/event.model'
import { SelectActiveEvent } from '../../../../reducers/event/event.actions'
import { SearchListItem } from '../../../../core/models/searchListItem'
import { Subscription } from 'rxjs'
import { responsesSelector } from '../../../../reducers/response/response.selectors'
import { Response } from '../../../../reducers/response/response.model'
import { Actions, ofType } from '@ngrx/effects';

@Component({
    selector: 'app-activate-response',
    templateUrl: './activate-response.component.html',
    styleUrls: [ './activate-response.component.scss' ],
    animations: [ fadeInOut ],
})
export class ActivateResponseComponent implements OnInit, OnDestroy {
    hover: boolean;
    active = false;
    disabled = false;
    selectedActionPlan: ActionPlan = null;
    showActionPlan = false;
    activeEvent: Event;
    activeResponse: Response;
    listItems: SearchListItem[];
    actionPlans: ActionPlan[];
    activated = false;
    loadingFullActionPlan = false;

    private actionPlansSub$: Subscription;
    private activeEventSub$: Subscription;
    private responsesSub$: Subscription;

    constructor (private store: Store<AppState>, private actions$: Actions) { }

    ngOnInit() {
        this.initSubscriptions();
        this.store.dispatch(new GetActionPlans())
    }

    ngOnDestroy() {
        this.actionPlansSub$.unsubscribe()
        this.activeEventSub$.unsubscribe()
        this.responsesSub$.unsubscribe()
    }

    initSubscriptions() {
        this.actionPlansSub$ = this.store
            .pipe(select(actionPlansSelector))
            .subscribe(actionPlans => {
                this.listItems = actionPlans.map(ap => ({
                    name: ap.name,
                    id: ap.actionPlanId,
                    icon: ap.icon || '',
                    color: ap.color || '',
                }))
                this.actionPlans = actionPlans
            })

        this.actionPlansSub$ = this.store
            .pipe(select(selectedActionPlanSelector))
            .subscribe(actionPlan => {
                if (actionPlan) {
                    this.selectedActionPlan = actionPlan
                    if (this.loadingFullActionPlan &&
                        actionPlan.openActions &&
                        actionPlan.openActions.length > 0) {
                        this.loadingFullActionPlan = false;
                    }
                } else {
                    this.showActionPlan = false;
                }
            })

        this.activeEventSub$ = this.actions$
            .pipe(ofType(ResponseActionTypes.ShowActivateResponse))
            .subscribe(({ payload: { event } }: ShowActivateResponse) => {
                this.active = true;
                this.activated = false;
                this.activeEvent = event;
                this.showActionPlan = false;
                this.selectedActionPlan = null;
                this.store.dispatch(new SetSelectingActionPlan({ isSelecting: this.active }))
            })

        this.responsesSub$ = this.store
            .pipe(select(responsesSelector))
            .subscribe(responses => {
                if (this.activeEvent && this.selectedActionPlan) {
                    this.activeResponse = responses.find(
                        response =>
                            // response.actionPlan.actionPlanId === this.selectedActionPlan.actionPlanId &&
                            response.primaryEventClusterId === this.activeEvent.eventClusterId
                    )
                }
            })
    }

    onActionPlanChange() {
        // console.log(this.selectedActionPlan);
    }

    selectActionPlan = (item: SearchListItem) => {
        let actionPlan: ActionPlan = null
        if (item) {
            actionPlan = this.actionPlans.find(ap => ap.actionPlanId === item.id)
            if (!actionPlan.openActions) {
                this.loadingFullActionPlan = true;
                this.store.dispatch(new GetActionPlan({ actionPlanId: actionPlan.actionPlanId }));
            }
            this.showActionPlan = true
        }

        this.store.dispatch(new SelectActionPlan({ actionPlan }))
    }

    activateActionPlan = () => {
        this.store.dispatch(
            new PostNewResponse({
                event: this.activeEvent,
                actionPlan: this.selectedActionPlan,
            })
        )
        this.store.dispatch(new PutActionPlan({ actionPlan: this.selectedActionPlan }));
        this.activated = true
        this.selectedActionPlan = {
            ...this.selectedActionPlan,
            openActions: this.selectedActionPlan.openActions.map(a => ({
                ...a,
                status: ActionPlanStatus.Complete,
            })),
        }
    }

    onReturnToMapClick() {
        this.toggleActive()
    }

    toggleActive() {
        if (this.disabled) { return; }

        if (this.active) {
            this.active = false
            this.activated = false
        } else {
            this.active = true;
        }

        this.store.dispatch(new SelectActionPlan({ actionPlan: null }));
        this.store.dispatch(new SelectActiveEvent({ event: null }));
        this.store.dispatch(new SetSelectingActionPlan({ isSelecting: this.active }));
    }
}
