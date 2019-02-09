import { Subject } from 'rxjs';

import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  ViewChild,
} from '@angular/core';

import {
  ActionChangeType,
  ActionPlanNotificationAction,
  ActionStatus,
  AddEditAction,
} from '../../../../../../reducers/action-plan/action-plan.model';

@Component({
  selector: 'app-notification-template',
  templateUrl: './notification-template.component.html',
  styleUrls: ['./notification-template.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NotificationTemplateComponent implements OnInit, AfterViewInit {
  @ViewChild('textarea') textarea: ElementRef;
  @ViewChild('editButton') editButton: ElementRef;

  @Input() context: ActionPlanNotificationAction;
  @Input() last: boolean;
  @Input() first: boolean;
  @Input() canEdit: boolean;
  @Input() actionChangeSubject: Subject<{
    addEditAction: AddEditAction;
    actionChangeType: ActionChangeType;
  }>;
  @Input() isCloseAction: boolean;

  notificationText: string;
  editing = false;
  adding = false;
  actionStatus = ActionStatus;

  ngOnInit() {
    this.notificationText = this.context.parameters.message;

    // notification already sent, un-editable
    if (this.context.status === ActionStatus.Success) {
      this.canEdit = false;
      return;
    }

    if (this.context.parameters.editing) {
      this.editing = true;
      this.adding = true;
    }

    if (this.canEdit && this.first && this.editButton) {
      this.editButton.nativeElement.focus();
    }
  }

  ngAfterViewInit() {
    if (this.context.parameters.editing) {
      this.textarea.nativeElement.scrollIntoView();
      this.textarea.nativeElement.focus();
      //   this.context.parameters.editing = false;
    }
  }

  addNew() {
    this.notificationText = '';
    this.adding = true;
  }

  remove() {
    const addEditAction: AddEditAction = { isRemoveAction: true };
    this.actionChangeSubject.next({
      addEditAction,
      actionChangeType: ActionChangeType.Delete,
    });
  }

  edit() {
    this.editing = true;
    setTimeout(() => {
      this.textarea.nativeElement.focus();
    }); // kick this event to the end of the line
  }

  editComplete() {
    this.editing = false;
    this.context.parameters.editing = false;
  }

  notificationChanged() {
    if (!this.canEdit) {
      return;
    }

    let actionChangeType = this.adding
      ? ActionChangeType.Add
      : ActionChangeType.Edit;
    if (this.notificationText.trim() === '') {
      actionChangeType = ActionChangeType.Delete;
    }

    const addEditAction: AddEditAction = {
      actionChangedString: actionChangeType,
      isCloseAction: this.isCloseAction,
      action: {
        actionId:
          actionChangeType === ActionChangeType.Delete ||
          actionChangeType === ActionChangeType.Edit
            ? this.context.actionId
            : null,
        actionType: this.context.actionType,
        isActive: this.context.isActive,
        description: this.context.description,
        parameters: {
          message: this.notificationText,
        },
      },
    };

    this.actionChangeSubject.next({ addEditAction, actionChangeType });
  }
}
