import uuid from 'uuid/v4';

import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
} from '@angular/core';

import {
  ActionChangeType,
  ActionPlanAction,
  ActionPlanNotificationAction,
  ActionPlanType,
  ActionStatus,
  AddEditAction,
} from '../../../../reducers/action-plan/action-plan.model';
import { Response } from '../../../../reducers/response/response.model';

@Component({
  selector: 'app-update-response',
  templateUrl: './update-response.component.html',
  styleUrls: ['./update-response.component.scss'],
})
export class UpdateResponseComponent implements OnInit, OnChanges {
  @Input() activeResponse: Response;

  @Output() onCancel = new EventEmitter();
  @Output() onActionsUpdated = new EventEmitter<{
    response: Response;
    actions: AddEditAction[];
  }>();

  modified = false;
  showSuccessMessage = false;
  updating = false;
  actionPlanActions: ActionPlanAction[];

  private _addEditActions = new Map<string, AddEditAction>();
  private _actionPlanActions: ActionPlanAction[];

  ngOnInit() {
    this._updateActions(false);
  }

  ngOnChanges() {
    this._updateActions(true);
  }

  onCancelClick() {
    this.modified = false;
    this.actionPlanActions = this._actionPlanActions; // revert to original array
    this._addEditActions.clear(); // clear any modifications
    this.onCancel.emit();
  }

  addEditAction(action: AddEditAction) {
    this._addEditActions.set(action.action.actionId, action);
    this.modified = true;
  }

  removeAction(action: AddEditAction) {
    this._addEditActions.delete(action.action.actionId);
    this.modified = true;
  }

  sendUpdates() {
    if (!this.modified) {
      return;
    }

    this.modified = false;
    this.updating = true;

    if (this._addEditActions.size > 0) {
      const actions = Array.from(this._addEditActions.values()).map(
        addEditAction => {
          if (addEditAction.actionChangedString === ActionChangeType.Add) {
            return {
              ...addEditAction,
              action: {
                ...addEditAction.action,
                actionId: '00000000-0000-0000-0000-000000000000', // must be null to add a new action, but needs a local ID to use Map object
              },
            };
          }
          return addEditAction;
        }
      );
      this.onActionsUpdated.emit({ actions, response: this.activeResponse });
      this._addEditActions.clear();
    }
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
        editing: true,
      },
    };

    this.actionPlanActions.push(notification);
  }

  private _getEditableActions() {
    if (
      this.activeResponse &&
      this.activeResponse.actionPlan &&
      this.activeResponse.actionPlan.openActions
    ) {
      return this.activeResponse.actionPlan.openActions.filter(action => {
        switch (action.actionType) {
          case ActionPlanType.Notification:
          case ActionPlanType.Twilio:
            return action.status !== ActionStatus.Success;
          case ActionPlanType.LightSensor:
            return true;
          case ActionPlanType.EmergencyCall:
          case ActionPlanType.Email:
          case ActionPlanType.None:
          default:
            return false;
        }
      });
    }
    return [];
  }

  private _updateActions(updateStatus: boolean) {
    if (this.activeResponse && this.activeResponse.actionPlan) {
      this.actionPlanActions = [...this._getEditableActions()];
      this._actionPlanActions = [...this._getEditableActions()];
      if (updateStatus) {
        this.showSuccessMessage =
          this.updating &&
          !this.actionPlanActions.some(
            action => action.status !== ActionStatus.Success
          );
      }
    }
  }
}
