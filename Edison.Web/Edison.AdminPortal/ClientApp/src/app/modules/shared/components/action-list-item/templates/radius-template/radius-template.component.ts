import { Component, EventEmitter, Input, OnChanges, OnInit } from '@angular/core';

import {
    ActionChangeType, ActionPlanColor, ActionPlanRadiusAction, ActionStatus, AddEditAction
} from '../../../../../../reducers/action-plan/action-plan.model';

@Component({
    selector: 'app-radius-template',
    templateUrl: './radius-template.component.html',
    styleUrls: [ './radius-template.component.scss' ]
})
export class RadiusTemplateComponent implements OnInit, OnChanges {
    @Input() context: ActionPlanRadiusAction;
    @Input() last: boolean;
    @Input() canEdit: boolean;
    @Input() onchange: EventEmitter<{ actionId: string, addEditAction: AddEditAction }>;
    @Input() isCloseAction: boolean;

    _color: string;
    actionPlanColors = Object.keys(ActionPlanColor).map(key => (ActionPlanColor[ key ]).toLowerCase());
    actionStatus = ActionStatus;

    ngOnInit(): void {
        this.updateLocalColor();
    }

    ngOnChanges(): void {
        this.updateLocalColor();
    }

    updateLocalColor() {
        this._color = this.context.parameters.color;
    }

    getBgColor() {
        return this._color.toLowerCase();
    }

    updateColor(color: string) {
        if (this._color === color) {
            this._color = 'off';
        } else {
            this._color = color;
        }

        const addEditAction: AddEditAction = {
            actionChangedString: ActionChangeType.Edit,
            isCloseAction: this.isCloseAction,
            action: {
                ...this.context,
                parameters: {
                    ...this.context.parameters,
                    color: this._color,
                }
            }
        };

        this.onchange.emit({
            actionId: this.context.actionId,
            addEditAction,
        });
    }
}
