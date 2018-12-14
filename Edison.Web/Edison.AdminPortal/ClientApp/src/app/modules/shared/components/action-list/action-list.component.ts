import {
    AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output
} from '@angular/core';

import {
    ActionPlanAction, AddEditAction
} from '../../../../reducers/action-plan/action-plan.model';

@Component({
    selector: 'app-action-list',
    templateUrl: './action-list.component.html',
    styleUrls: [ './action-list.component.scss' ]
})
export class ActionListComponent implements AfterViewInit {

    constructor (private _element: ElementRef) { }

    @Input() actions: ActionPlanAction[];
    @Input() canEdit: boolean = false;
    @Input() isCloseActionList: boolean;
    @Output() onchange = new EventEmitter<AddEditAction>();

    ngAfterViewInit() {
        this._element.nativeElement.focus();
    }

    onItemChange(updateableAction: AddEditAction) {
        this.onchange.emit(updateableAction);
    }
}
