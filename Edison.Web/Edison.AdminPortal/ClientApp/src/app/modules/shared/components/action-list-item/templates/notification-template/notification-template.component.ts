import {
    AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, EventEmitter, Input, OnInit,
    ViewChild
} from '@angular/core';

import {
    ActionChangeType, ActionPlanNotificationAction, ActionStatus, AddEditAction
} from '../../../../../../reducers/action-plan/action-plan.model';

@Component({
    selector: 'app-notification-template',
    templateUrl: './notification-template.component.html',
    styleUrls: [ './notification-template.component.scss' ],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NotificationTemplateComponent implements OnInit, AfterViewInit {
    @ViewChild('textarea') textarea: ElementRef;
    @ViewChild('editButton') editButton: ElementRef;

    @Input() context: ActionPlanNotificationAction;
    @Input() last: boolean;
    @Input() first: boolean;
    @Input() canEdit: boolean;
    @Input() onchange: EventEmitter<{ actionId: string, addEditAction: AddEditAction }>;

    notificationText: string;
    editing = false;
    adding = false;
    actionStatus = ActionStatus;

    ngOnInit(): void {
        this.notificationText = this.context.parameters.message;
        if (this.context.parameters.editing) {
            this.editing = true;
            this.adding = true;
        } else {
            this.editing = false;
            this.adding = false;
        }

        if (this.canEdit && this.first) {
            this.editButton.nativeElement.focus();
        }
    }

    ngAfterViewInit() {
        if (this.context.parameters.editing) {
            this.textarea.nativeElement.scrollIntoView();
            this.textarea.nativeElement.focus();
            this.context.parameters.editing = false;
        }
    }

    addNew() {
        this.notificationText = '';
        this.adding = true;
    }

    remove() {
        const addEditAction: AddEditAction = { isRemoveAction: true };

        this.onchange.emit({
            actionId: this.context.actionId,
            addEditAction,
        });
    }

    edit() {
        this.editing = true;
        setTimeout(() => { this.textarea.nativeElement.focus(); }); // kick this event to the end of the line
    }

    editComplete() {
        this.editing = false;
    }

    notificationChanged() {
        if (!this.canEdit) {
            return;
        }

        let actionChangeType = this.adding ? ActionChangeType.Add : ActionChangeType.Edit;
        if (this.notificationText.trim() === '') {
            actionChangeType = ActionChangeType.Delete;
        }

        const addEditAction: AddEditAction = {
            actionChangedString: actionChangeType,
            isCloseAction: true,
            action: {
                actionId: actionChangeType === ActionChangeType.Delete || actionChangeType === ActionChangeType.Edit ? this.context.actionId : null,
                actionType: this.context.actionType,
                isActive: this.context.isActive,
                description: this.context.description,
                parameters: {
                    message: this.notificationText
                }
            }
        };

        this.onchange.emit({
            actionId: this.context.actionId,
            addEditAction,
        });
    }
}
