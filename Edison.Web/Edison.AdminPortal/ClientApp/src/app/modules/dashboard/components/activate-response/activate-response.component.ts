import { Subscription } from 'rxjs';

import { Component, OnDestroy, OnInit } from '@angular/core';
import { Actions, ofType } from '@ngrx/effects';
import { select, Store } from '@ngrx/store';

import { fadeInOut } from '../../../../core/animations/fadeInOut';
import { SearchListItem } from '../../../../core/models/searchListItem';
import { AppState } from '../../../../reducers';
import {
    GetActionPlan, GetActionPlans, PutActionPlan, SelectActionPlan, SetSelectingActionPlan
} from '../../../../reducers/action-plan/action-plan.actions';
import { ActionPlan, ActionPlanStatus } from '../../../../reducers/action-plan/action-plan.model';
import {
    actionPlansSelector, selectedActionPlanSelector
} from '../../../../reducers/action-plan/action-plan.selectors';
import { SelectActiveEvent } from '../../../../reducers/event/event.actions';
import { Event } from '../../../../reducers/event/event.model';
import { activeEventSelector, eventsSelector } from '../../../../reducers/event/event.selectors';
import {
    PostNewResponse, ResponseActionTypes, ShowActivateResponse
} from '../../../../reducers/response/response.actions';
import { Response } from '../../../../reducers/response/response.model';
import { responsesSelector } from '../../../../reducers/response/response.selectors';

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
            .subscribe(({ payload: { event, actionPlanId } }: ShowActivateResponse) => {
                this.active = true;
                this.activated = false;
                this.activeEvent = event;
                this.showActionPlan = false;
                this.selectedActionPlan = null;

                if (actionPlanId) { this._selectActionPlan(actionPlanId); }

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
        if (item) {
            this._selectActionPlan(item.id);
        } else {
            this.store.dispatch(new SelectActionPlan({ actionPlan: null }))
        }
    }

    private _selectActionPlan(actionPlanId: string) {
        const actionPlan = this.actionPlans.find(ap => ap.actionPlanId === actionPlanId)
        if (!actionPlan.openActions) {
            this.loadingFullActionPlan = true;
            this.store.dispatch(new GetActionPlan({ actionPlanId: actionPlan.actionPlanId }));
        }
        this.showActionPlan = true

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
