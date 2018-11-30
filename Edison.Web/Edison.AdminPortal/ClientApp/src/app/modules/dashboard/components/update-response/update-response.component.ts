import { Subscription } from 'rxjs';
import uuid from 'uuid/v4';

import {
    Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output
} from '@angular/core';
import { Actions, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';

import { AppState } from '../../../../reducers';
import {
    ActionChangeType, ActionPlanAction, ActionPlanNotificationAction, ActionPlanType, AddEditAction
} from '../../../../reducers/action-plan/action-plan.model';
import {
    ResponseActionTypes, UpdateResponseActions
} from '../../../../reducers/response/response.actions';
import { Response } from '../../../../reducers/response/response.model';

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
    _openActions: ActionPlanAction[];

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
            this._openActions = [ ...this.activeResponse.actionPlan.openActions ];
        }
    }

    onCancel() {
        this.cancel.emit()
        this.removeEmptyopenActions();
    }

    removeEmptyopenActions() {
        const actionsToRemove = this._openActions.filter(ca => !ca.parameters.message || ca.parameters.message.trim() === '')

        // remove from local arr
        this._openActions = this._openActions.filter(ca => !actionsToRemove.some(atr => atr.actionId === ca.actionId));

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

    updated({ addEditAction, actionId }: { addEditAction: AddEditAction, actionId: string }) {
        if (addEditAction && !addEditAction.isRemoveAction) {
            this.updateAddEditActions(addEditAction, actionId);
            this.modified = true
        } else {
            this.addEditActions.delete(actionId);
            this._openActions = this._openActions.filter(oa => oa.actionId !== actionId);
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

        this._openActions.push(notification);
        this.updateAddEditActions({
            actionChangedString: ActionChangeType.Add,
            isCloseAction: true,
            action: notification
        }, tempActionId);
    }
}
