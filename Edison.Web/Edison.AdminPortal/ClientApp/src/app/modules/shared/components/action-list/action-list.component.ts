import { Component, Input, Output, EventEmitter } from '@angular/core'
import { ActionPlanAction, AddEditAction } from '../../../../reducers/action-plan/action-plan.model'

@Component({
    selector: 'app-action-list',
    templateUrl: './action-list.component.html',
    styleUrls: [ './action-list.component.scss' ]
})
export class ActionListComponent {
    @Input() actions: ActionPlanAction[];
    @Input() canEdit: boolean = false;
    @Output() onchange = new EventEmitter<AddEditAction>();

    onItemChange(updateableAction: AddEditAction) {
        this.onchange.emit(updateableAction);
    }
}
