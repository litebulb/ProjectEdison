import { Observable, Subscription } from 'rxjs';

import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material';
import { select, Store } from '@ngrx/store';

import { ConfirmDialogData } from '../../../../core/models/confirmDialogData';
import { AppState } from '../../../../reducers';
import { ActionPlan } from '../../../../reducers/action-plan/action-plan.model';
import { actionPlansSelector } from '../../../../reducers/action-plan/action-plan.selectors';
import {
    EndConversation, SendNewMessage, SendUserReadReceipt, ToggleUserChatWindow
} from '../../../../reducers/chat/chat.actions';
import { Message } from '../../../../reducers/chat/chat.model';
import {
    chatActiveMessagesSelector, chatActiveUserSelector
} from '../../../../reducers/chat/chat.selectors';
import { ShowEvents } from '../../../../reducers/event/event.actions';
import { Event, EventInstance } from '../../../../reducers/event/event.model';
import { activeMobileEventsSelector } from '../../../../reducers/event/event.selectors';
import { ShowActivateResponse } from '../../../../reducers/response/response.actions';
import {
    ConfirmDialogComponent
} from '../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
    selector: 'app-user-chat',
    templateUrl: './user-chat.component.html',
    styleUrls: [ './user-chat.component.scss' ],
})
export class UserChatComponent implements OnInit, OnDestroy {
    @ViewChild('textarea') textareaRef: ElementRef;

    messagesSub$: Subscription;
    messages: Message[];
    activeUserId$: Subscription;
    actionPlans$: Observable<ActionPlan[]>;
    activeMobileEvents$: Subscription;
    message: string
    userId: string;
    userName: string;
    event: Event;
    latestEventInstance: EventInstance;

    constructor (private store: Store<AppState>, public dialog: MatDialog) { }

    ngOnInit() {
        this.messagesSub$ = this.store
            .pipe(select(chatActiveMessagesSelector))
            .subscribe(messages => {
                const userChanged = this.messages && this.messages
                    .filter(msg => msg.role !== 'admin')
                    .some(msg => msg.name !== this.userName);
                if (!this.messages || this.messages.length < messages.length || userChanged) {
                    this.messages = messages;
                    if (this.userId) {
                        this.store.dispatch(new SendUserReadReceipt({ userId: this.userId, date: new Date() }))
                    }
                }
            });
        this.activeUserId$ = this.store
            .pipe(select(chatActiveUserSelector))
            .subscribe(user => {
                if (this.userId !== user.userId) {
                    this.userId = user.userId;
                    this.userName = user.name;

                    this.store.dispatch(new SendUserReadReceipt({ userId: user.userId, date: new Date() }))
                }
            });
        this.actionPlans$ = this.store.pipe(select(actionPlansSelector));
        this.activeMobileEvents$ = this.store
            .pipe(select(activeMobileEventsSelector))
            .subscribe(events => {
                this.event = events.find(event => event.events.some(ee => ee.metadata.userId === this.userId));
                if (this.event) {
                    this.latestEventInstance = this.event.events
                        .sort((a, b) => (new Date(b.date).getTime() - new Date(a.date).getTime()))[ 0 ]
                }
            });

        this.textareaRef.nativeElement.focus();
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

    showOnMap() {
        this.store.dispatch(new ShowEvents({ events: [ this.event ] }));
    }

    close() {
        this.store.dispatch(new ToggleUserChatWindow({ open: false }));
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

    activateResponse = () => {
        this.store.dispatch(new ShowActivateResponse({
            event: this.event,
            actionPlanId: this.latestEventInstance.metadata.reportType
        }));
        this.close();
    }

    private endChat() {
        this.store.dispatch(new EndConversation({ userId: this.userId }));
    }
}
