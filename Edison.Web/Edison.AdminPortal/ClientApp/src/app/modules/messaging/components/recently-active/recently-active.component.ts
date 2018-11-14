import { Component, OnInit } from '@angular/core'
import { Store, select } from '@ngrx/store'
import { AppState } from '../../../../reducers'
import { SetPageData } from '../../../../reducers/app/app.actions'
import { Observable } from 'rxjs';
import { chatActiveUsersSelector } from '../../../../reducers/chat/chat.selectors';
import { ToggleAllUsersChatWindow, SelectActiveConversation, ToggleUserChatWindow } from '../../../../reducers/chat/chat.actions';

@Component({
    selector: 'app-recently-active',
    templateUrl: './recently-active.component.html',
    styleUrls: [ './recently-active.component.scss' ],
})
export class RecentlyActiveComponent implements OnInit {
    activeUsers$: Observable<any[]>;

    constructor (private store: Store<AppState>) { }

    ngOnInit() {
        this.store.dispatch(new SetPageData({ title: 'Messaging', sidebar: true }));
        this.activeUsers$ = this.store.pipe(select(chatActiveUsersSelector));
    }

    selectActiveUser(user) {
        this.store.dispatch(new ToggleUserChatWindow({ open: true, userId: user.id, userName: user.name }));
        this.store.dispatch(new SelectActiveConversation({ conversationId: user.conversationId }));
    }

    selectAllChat() {
        this.store.dispatch(new ToggleAllUsersChatWindow({ open: true, userId: '*' }));
    }
}
