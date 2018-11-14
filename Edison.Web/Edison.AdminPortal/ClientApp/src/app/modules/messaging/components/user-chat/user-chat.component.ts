import { Component, OnInit, OnDestroy } from '@angular/core'
import { Observable, Subscription } from 'rxjs';
import { Store, select } from '@ngrx/store';
import { AppState } from '../../../../reducers';
import { chatActiveMessagesSelector, chatActiveUserSelector } from '../../../../reducers/chat/chat.selectors';
import { ToggleAllUsersChatWindow, SendNewMessage, EndConversation } from '../../../../reducers/chat/chat.actions';
import { actionPlansSelector } from '../../../../reducers/action-plan/action-plan.selectors';
import { ActionPlan } from '../../../../reducers/action-plan/action-plan.model';
import { Message } from '../../../../reducers/chat/chat.model';
import { MatDialog } from '@angular/material';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ConfirmDialogData } from '../../../../core/models/confirmDialogData';

@Component({
    selector: 'app-user-chat',
    templateUrl: './user-chat.component.html',
    styleUrls: [ './user-chat.component.scss' ],
})
export class UserChatComponent implements OnInit, OnDestroy {
    messagesSub$: Subscription;
    messages: Message[];
    activeUserId$: Subscription;
    actionPlans$: Observable<ActionPlan[]>;
    message: string
    userId: string;
    userName: string;

    constructor (private store: Store<AppState>, public dialog: MatDialog) { }

    ngOnInit() {
        this.messagesSub$ = this.store.pipe(select(chatActiveMessagesSelector)).subscribe(messages => {
            this.messages = messages;
        });
        this.activeUserId$ = this.store.pipe(select(chatActiveUserSelector)).subscribe(user => {
            this.userId = user.userId;
            this.userName = user.name;
        });
        this.actionPlans$ = this.store.pipe(select(actionPlansSelector));
    }

    ngOnDestroy() {
        this.activeUserId$.unsubscribe();
        this.messagesSub$.unsubscribe();
    }

    onEnter(event) {
        if (event.keyCode !== 13) {
            return
        }

        this.store.dispatch(new SendNewMessage({ message: this.message, userId: this.userId }));
        this.messages.push({
            name: 'YOU',
            text: this.message,
            role: 'admin',
            self: true,
        });
        this.message = '';

        event.preventDefault()
        return false
    }

    close() {
        this.store.dispatch(new ToggleAllUsersChatWindow({ open: false }));
    }

    confirmEndChat() {
        const data: ConfirmDialogData = {
            defaultText: 'Are you sure you want to end this conversation?'
        };
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            width: '250px',
            data
        });

        dialogRef.afterClosed().subscribe((result: boolean) => {
            if (result) {
                this.endChat();
                this.close();
            }
        });
    }

    private endChat() {
        this.store.dispatch(new EndConversation({ userId: this.userId }));
    }
}
