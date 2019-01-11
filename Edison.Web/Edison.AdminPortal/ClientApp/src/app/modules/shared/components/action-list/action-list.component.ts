import { Subject, Subscription } from 'rxjs';

import {
    AfterViewInit, Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output
} from '@angular/core';

import {
    ActionChangeType, ActionPlanAction, AddEditAction
} from '../../../../reducers/action-plan/action-plan.model';

@Component({
    selector: 'app-action-list',
    templateUrl: './action-list.component.html',
    styleUrls: [ './action-list.component.scss' ]
})
export class ActionListComponent implements AfterViewInit, OnInit, OnDestroy {

    constructor (private _element: ElementRef) { }

    @Input() actions: ActionPlanAction[];
    @Input() canEdit: boolean = false;
    @Input() isCloseActionList: boolean;

    @Output() onRemoveAction = new EventEmitter<AddEditAction>();
    @Output() onAddAction = new EventEmitter<AddEditAction>();
    @Output() onEditAction = new EventEmitter<AddEditAction>();

    actionChangeSubject: Subject<{ addEditAction: AddEditAction, actionChangeType: ActionChangeType }>;

    private _actionChange$: Subscription;

    ngOnInit() {
        this.actionChangeSubject = new Subject<{ addEditAction: AddEditAction, actionChangeType: ActionChangeType }>();
        this._actionChange$ = this.actionChangeSubject.subscribe((action) => this._onActionChange(action));
    }

    ngOnDestroy() { this._actionChange$.unsubscribe(); }

    ngAfterViewInit() { this._element.nativeElement.focus(); }

    private _onActionChange({ addEditAction, actionChangeType }: { addEditAction: AddEditAction, actionChangeType: ActionChangeType }) {
        switch (actionChangeType) {
            case ActionChangeType.Add:
                this.onAddAction.emit(addEditAction);
                break;
            case ActionChangeType.Edit:
                this.onEditAction.emit(addEditAction);
                break;
            case ActionChangeType.Delete:
                this.onRemoveAction.emit(addEditAction);
                break;
        }
    }
}
