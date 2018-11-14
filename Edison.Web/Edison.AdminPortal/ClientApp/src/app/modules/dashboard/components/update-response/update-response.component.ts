import { Component, Input, Output, EventEmitter, OnInit, OnDestroy, OnChanges } from '@angular/core'
import { Response } from '../../../../reducers/response/response.model'
import { AddEditAction, ActionPlanAction, ActionPlanNotificationAction, ActionPlanType, ActionChangeType } from '../../../../reducers/action-plan/action-plan.model';
import { Store } from '@ngrx/store';
import { AppState } from '../../../../reducers';
import { UpdateResponseActions, ResponseActionTypes } from '../../../../reducers/response/response.actions';
import { Subscription } from 'rxjs';
import uuid from 'uuid/v4';
import { Actions, ofType } from '@ngrx/effects';

@Component({
    selector: 'app-update-response',
    templateUrl: './update-response.component.html',
    styleUrls: [ './update-response.component.scss' ],
})
export class UpdateResponseComponent implements OnInit, OnDestroy, OnChanges {
    @Input()
    activeResponse: Response

    @Output()
    cancel = new EventEmitter()

    @Output()
    change = new EventEmitter<AddEditAction>();

    successSub$: Subscription;

    updateSucceeded = false
    modified = false
    addEditActions = new Map<string, AddEditAction>();
    _closeActions: ActionPlanAction[];

    constructor (private store: Store<AppState>, private updates$: Actions) { }

    ngOnInit(): void {
        this.updateActions();

        this.successSub$ = this.updates$
            .pipe(ofType(ResponseActionTypes.UpdateResponseActionsSuccess))
            .subscribe(() => this.updateSucceeded = true)
    }

    ngOnDestroy(): void {
        this.successSub$.unsubscribe();
    }

    ngOnChanges() {
        this.updateActions();
    }

    updateActions() {
        if (this.activeResponse && this.activeResponse.actionPlan) {
            this._closeActions = [ ...this.activeResponse.actionPlan.closeActions ];
        }
    }

    onCancel() {
        this.cancel.emit()
        this.removeEmptyCloseActions();
    }

    removeEmptyCloseActions() {
        const actionsToRemove = this._closeActions.filter(ca => !ca.parameters.message || ca.parameters.message.trim() === '')

        // remove from local arr
        this._closeActions = this._closeActions.filter(ca => !actionsToRemove.some(atr => atr.actionId === ca.actionId));

        actionsToRemove.forEach(actionToRemove => {
            const addEditAction = this.addEditActions.get(actionToRemove.actionId);
            if (addEditAction) {
                switch (addEditAction.actionChangedString) {
                    case ActionChangeType.Add: {
                        // someone cancelled out of adding a new notification
                        this.addEditActions.delete(actionToRemove.actionId);
                        break;
                    }
                    case ActionChangeType.Edit: {
                        // a pre-existing notification was blanked out (aka removed)
                        this.addEditActions.set(actionToRemove.actionId, {
                            actionChangedString: ActionChangeType.Delete,
                            isCloseAction: true,
                            action: actionToRemove
                        })
                        break;
                    }
                }
            }
        })
    }

    onUpdate() {
        if (!this.modified) {
            return
        }

        this.modified = false
        this.updateSucceeded = false;

        if (this.addEditActions.size > 0) {
            this.updateResponseActions(Array.from(this.addEditActions.values()));
            this.addEditActions.clear();
        } else {
            this.updateSucceeded = true
        }
    }

    updated({ addEditAction, actionId }) {
        this.modified = true

        if (addEditAction) {
            this.updateAddEditActions(addEditAction, actionId);
        }
    }

    updateAddEditActions(addEditAction: AddEditAction, actionId) {
        this.addEditActions.set(actionId, addEditAction);
    }

    updateResponseActions(actions: AddEditAction[]) {
        this.store.dispatch(new UpdateResponseActions({
            response: this.activeResponse,
            actions
        }))
    }

    addNotification() {
        const tempActionId = uuid();
        const notification: ActionPlanNotificationAction = {
            actionId: tempActionId,
            actionType: ActionPlanType.Notification,
            isActive: true,
            description: 'Mobile App Notification will be sent',
            parameters: {
                message: '',
                editing: true
            }
        }

        this._closeActions.push(notification);
        this.updateAddEditActions({
            actionChangedString: ActionChangeType.Add,
            isCloseAction: true,
            action: notification
        }, tempActionId);
    }
}
