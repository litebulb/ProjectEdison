import { Component, EventEmitter, Input, OnChanges, OnInit, Output } from '@angular/core';

import { fadeInOut } from '../../../../core/animations/fadeInOut';
import { SearchListItem } from '../../../../core/models/searchListItem';
import {
    ActionPlan, ActionPlanAction, ActionPlanType, ActionStatus
} from '../../../../reducers/action-plan/action-plan.model';
import { Event } from '../../../../reducers/event/event.model';
import { Response } from '../../../../reducers/response/response.model';

@Component({
    selector: 'app-activate-response',
    templateUrl: './activate-response.component.html',
    styleUrls: [ './activate-response.component.scss' ],
    animations: [ fadeInOut ],
})
export class ActivateResponseComponent implements OnInit, OnChanges {
    @Input() actionPlans: ActionPlan[];
    @Input() actionPlan: ActionPlan;
    @Input() event: Event;
    @Input() activeResponse: Response;

    @Output() onSelectActionPlan = new EventEmitter<ActionPlan>();
    @Output() onGetFullActionPlan = new EventEmitter<string>();
    @Output() onPostNewResponse = new EventEmitter();
    @Output() onRetry = new EventEmitter();
    @Output() onClose = new EventEmitter();
    @Output() onOpen = new EventEmitter();

    activated = false;
    active = false;
    activeEvent: Event;
    disabled = false;
    hover: boolean;
    listItems: SearchListItem[];
    loadingFullActionPlan = false;
    responseNeedsLocation: boolean;
    showActionPlan = false;

    actionPlanPartialSuccess: boolean;
    actionPlanSuccessful: boolean;
    actionPlanHasErrors: boolean;
    actionPlanNeedsLocation: boolean;

    ngOnInit() {
        this._initActionPlanItems();
        this._initSelectedActionPlan();
        this._initActionPlanFromResponse();

        this._updateResponseStatus();
    }

    ngOnChanges() {
        this._initActionPlanItems();
        this._initActionPlanFromResponse();
        this._initSelectedActionPlan();
    }

    private _initActionPlanItems() {
        this.listItems = this.actionPlans.map(ap => ({
            name: ap.name,
            id: ap.actionPlanId,
            icon: ap.icon || '',
            color: ap.color || '',
        }));
    }

    private _initSelectedActionPlan() {
        if (this.actionPlan) {
            if (this.loadingFullActionPlan &&
                this.actionPlan.openActions &&
                this.actionPlan.openActions.length > 0) {
                this.loadingFullActionPlan = false;
            }
            this.responseNeedsLocation = !this.activeEvent;
        } else {
            this.showActionPlan = false;
        }
    }

    private _initActionPlanFromResponse() {
        if (this.activeResponse) {
            this.actionPlan = this.activeResponse.actionPlan;
            this._updateResponseStatus();
        }
    }

    private _updateResponseStatus() {
        if (this.actionPlan && this.actionPlan.openActions) {
            this.actionPlanSuccessful = !this.actionPlan.openActions.some(action => action.status !== ActionStatus.Success);
            this.actionPlanNeedsLocation = this.responseNeedsLocation && this.actionPlan.openActions.some(action => action.status === ActionStatus.Skipped);
            this.actionPlanHasErrors = this.actionPlan.openActions.some(action => action.status === ActionStatus.Error || action.status === ActionStatus.Unknown);
            this.actionPlanPartialSuccess = this.actionPlan.openActions.some(action => action.status === ActionStatus.Success);
        } else {
            this.actionPlanSuccessful = false;
            this.actionPlanNeedsLocation = false;
            this.actionPlanHasErrors = false;
            this.actionPlanPartialSuccess = false;
        }
    }

    onActionPlanChange() {
        // console.log(this.selectedActionPlan);
    }

    selectActionPlan = (item: SearchListItem) => {
        if (item) {
            this._selectActionPlan(item.id);
        } else {
            this.onSelectActionPlan.emit(null);
        }
    }

    private _selectActionPlan(actionPlanId: string) {
        const actionPlan = this.actionPlans.find(ap => ap.actionPlanId === actionPlanId)
        if (!actionPlan.openActions) {
            this.loadingFullActionPlan = true;
            this.onGetFullActionPlan.emit(actionPlanId);
        } else if (!this.activeEvent) {
            this.responseNeedsLocation = true;
        }

        this.showActionPlan = true
        this.onSelectActionPlan.emit(actionPlan);
    }

    private _setActionsLoading = (actions: ActionPlanAction[], locationActionsOnly?: boolean) => {
        if (actions) {
            return actions
                .filter(action => action.status !== ActionStatus.Success &&
                    (!locationActionsOnly || action.actionType === ActionPlanType.LightSensor))
                .map(action => ({
                    ...action,
                    loading: true
                }));
        }
        return actions;
    }

    activateActionPlan = () => {
        this.actionPlan = {
            ...this.actionPlan,
            openActions: this._setActionsLoading(this.actionPlan.openActions),
        }
        this.onPostNewResponse.emit({ event: this.activeEvent, actionPlan: this.actionPlan });
        this.activated = true;
    }

    retry() {
        if (this.activeResponse) {
            this.onRetry.emit(this.activeResponse.responseId);
        }
    }

    onReturnToMapClick() {
        this.close();
    }

    close() {
        this.active = false;
        this.activated = false;
        this.onSelectActionPlan.emit(null);
        this.onClose.emit();
    }

    open() {
        if (this.disabled) { return; }

        this.active = true;
        this.onSelectActionPlan.emit(null);
        this.onOpen.emit();
    }

    toggleActive() {
        if (this.active) {
            this.close();
        } else {
            this.open();
        }
    }
}
