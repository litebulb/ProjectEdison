import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { MatDialog } from '@angular/material';

import { ConfirmDialogData } from '../../../../core/models/confirmDialogData';
import { ActionPlan } from '../../../../reducers/action-plan/action-plan.model';
import { Message } from '../../../../reducers/chat/chat.model';
import { Event, EventInstance } from '../../../../reducers/event/event.model';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-user-chat',
  templateUrl: './user-chat.component.html',
  styleUrls: ['./user-chat.component.scss'],
})
export class UserChatComponent implements OnInit {
  @ViewChild('textarea') textareaRef: ElementRef;

  @Input() messages: Message[];
  @Input() actionPlans: ActionPlan[];
  @Input() activeEvent: Event;
  @Input() activeEventInstance: EventInstance;
  @Input() userName: string;
  @Input() userId: string;

  @Output() newMessage = new EventEmitter<{
    message: string;
    userId: string;
  }>();
  @Output() closeClick = new EventEmitter();
  @Output() endChatClick = new EventEmitter();
  @Output() activateResponseClick = new EventEmitter();
  @Output() showEvent = new EventEmitter();

  message: string;

  constructor(public dialog: MatDialog) {}

  ngOnInit() {
    this.textareaRef.nativeElement.focus();
  }

  onEnter(event) {
    if (event.keyCode !== 13) {
      return;
    }

    this.newMessage.emit({ message: this.message, userId: this.userId });
    this.messages.push({
      name: 'YOU',
      text: this.message,
      role: 'admin',
      self: true,
    });
    this.message = '';

    event.preventDefault();
    return false;
  }

  showOnMap() {
    this.showEvent.emit();
  }

  close() {
    this.closeClick.emit();
  }

  confirmEndChat() {
    const data: ConfirmDialogData = {
      defaultText: 'Are you sure you want to end this conversation?',
    };
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '250px',
      data,
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        this._endChat();
        this.close();
      }
    });
  }

  activateResponse = () => {
    this.activateResponseClick.emit();
    this.close();
  };

  private _endChat() {
    this.endChatClick.emit();
  }
}
