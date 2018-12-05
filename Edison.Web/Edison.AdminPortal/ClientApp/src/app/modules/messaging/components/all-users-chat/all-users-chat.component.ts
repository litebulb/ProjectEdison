import { Observable, Subscription } from 'rxjs';

import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { select, Store } from '@ngrx/store';

import { AppState } from '../../../../reducers';
import { SendNewMessage, ToggleAllUsersChatWindow } from '../../../../reducers/chat/chat.actions';
import { Message } from '../../../../reducers/chat/chat.model';
import {
    chatActiveUsersCountSelector, chatAllMessagesSelector
} from '../../../../reducers/chat/chat.selectors';
import { ShowActivateResponse } from '../../../../reducers/response/response.actions';

@Component({
    selector: 'app-all-users-chat',
    templateUrl: './all-users-chat.component.html',
    styleUrls: [ './all-users-chat.component.scss' ],
})
export class AllUsersChatComponent implements OnInit {
    @ViewChild('activateResponseButton') activateResponseButton: ElementRef;

    messagesSub$: Subscription;
    messages: Message[];
    userCount$: Observable<number>;
    message: string;

    constructor (private store: Store<AppState>) { }

    ngOnInit() {
        this.messagesSub$ = this.store.pipe(select(chatAllMessagesSelector)).subscribe(messages => {
            this.messages = messages;
        });
        this.userCount$ = this.store.pipe(select(chatActiveUsersCountSelector));

        this.activateResponseButton.nativeElement.focus();
    }

    showActivateResponse() {
        this.store.dispatch(new ShowActivateResponse({ event: null }));
    }

    onEnter(event) {
        if (event.keyCode !== 13) {
            return;
        }

        this.store.dispatch(new SendNewMessage({ message: this.message, userId: '*' }));
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

    close() {
        this.store.dispatch(new ToggleAllUsersChatWindow({ open: false }));
    }
}
