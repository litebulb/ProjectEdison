import {
    Component,
    Input,
    EventEmitter,
    OnInit,
    OnChanges,
} from '@angular/core'
import {
    ActionPlanRadiusAction,
    ActionPlanColor,
    ActionChangeType,
    AddEditAction,
} from '../../../../../../reducers/action-plan/action-plan.model'

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

    _color: string;
    actionPlanColors = Object.keys(ActionPlanColor).map(key => ActionPlanColor[ key ]);

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
            isCloseAction: true,
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
