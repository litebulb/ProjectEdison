import { Observable } from 'rxjs';

import { Component, OnInit } from '@angular/core';
import { select, Store } from '@ngrx/store';

import { ActiveUser } from '../../../../core/models/activeUser';
import { AppState } from '../../../../reducers';
import { ActionPlan } from '../../../../reducers/action-plan/action-plan.model';
import { actionPlansSelector } from '../../../../reducers/action-plan/action-plan.selectors';
import { AppPage, SetPageData } from '../../../../reducers/app/app.actions';
import {
    SelectActiveConversation, ToggleAllUsersChatWindow, ToggleUserChatWindow
} from '../../../../reducers/chat/chat.actions';
import { Chat } from '../../../../reducers/chat/chat.model';
import {
    chatActiveUserSelector, chatActiveUsersSelector, chatMessagesSelector
} from '../../../../reducers/chat/chat.selectors';
import { Event } from '../../../../reducers/event/event.model';
import { activeMobileEventsSelector } from '../../../../reducers/event/event.selectors';

@Component({
    selector: 'app-messaging',
    templateUrl: './messaging.component.html',
    styleUrls: [ './messaging.component.scss' ]
})
export class MessagingComponent implements OnInit {
    activeUsers$: Observable<ActiveUser[]>;
    actionPlans$: Observable<ActionPlan[]>;
    messages$: Observable<Chat[]>;
    activeUser$: Observable<{ name: string, userId: string }>;
    activeMobileEvents$: Observable<Event[]>;

    constructor (private store: Store<AppState>) { }

    ngOnInit() {
        this.store.dispatch(new SetPageData({ title: AppPage.Messaging, showDownArrow: true, showReloadButton: true }));

        this._setupObservables();
    }

    openAllChat() {
        this.store.dispatch(new ToggleAllUsersChatWindow({ open: true, userId: '*' }));
    }

    selectActiveUser(user: ActiveUser) {
        this.store.dispatch(new ToggleUserChatWindow({ open: true, userId: user.id, userName: user.name }));
        this.store.dispatch(new SelectActiveConversation({ conversationId: user.conversationId }));
    }

    private _setupObservables() {
        this.activeUsers$ = this.store.pipe(select(chatActiveUsersSelector));
        this.actionPlans$ = this.store.pipe(select(actionPlansSelector));
        this.messages$ = this.store.pipe(select(chatMessagesSelector));
        this.activeMobileEvents$ = this.store.pipe(select(activeMobileEventsSelector));
        this.activeUser$ = this.store.pipe(select(chatActiveUserSelector));
    }

}
