import { Observable, of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { Action } from '@ngrx/store';

import { environment } from '../../environments/environment';
import { DirectlineService } from '../core/services/directline/directline.service';
import {
    ChatActionTypes, EndConversation, GetChatAuthTokenError, GetChatAuthTokenSuccess,
    SelectActiveUser, SendNewMessage, SendUserReadReceipt, ToggleUserChatWindow
} from '../reducers/chat/chat.actions';

@Injectable()
export class ChatEffects {
    @Effect()
    getAuthToken$: Observable<Action> = this.actions$.pipe(
        ofType(ChatActionTypes.GetChatAuthToken),
        mergeMap(
            action => this.http.get(`${environment.baseUrl}${environment.chatAuthUrl}`).pipe(
                map(
                    result => result ? new GetChatAuthTokenSuccess({ result }) : new GetChatAuthTokenError()
                ),
                catchError(() => of(new GetChatAuthTokenError()))
            )
        )
    );

    @Effect({ dispatch: false })
    endConversation$: Observable<void> = this.actions$.pipe(
        ofType(ChatActionTypes.EndConversation),
        map(({ payload: { userId } }: EndConversation) => {
            this.directlineService.endConversation(userId);
            return null;
        })
    )

    @Effect()
    sendNewMessage$: Observable<Action> = this.actions$.pipe(
        ofType(ChatActionTypes.SendNewMessage),
        map(({ payload: { message, userId } }: SendNewMessage) => {
            this.directlineService.sendMessage(message, userId);
            return new SendUserReadReceipt({ userId, date: new Date() })
        })
    )

    @Effect()
    openChatWindow$: Observable<Action> = this.actions$.pipe(
        ofType(ChatActionTypes.ToggleAllUsersChatWindow, ChatActionTypes.ToggleUserChatWindow),
        map(({ payload }: ToggleUserChatWindow) => new SelectActiveUser(payload))
    )

    @Effect({ dispatch: false })
    sendUserReadReceipt$: Observable<void> = this.actions$.pipe(
        ofType(ChatActionTypes.SendUserReadReceipt),
        map(({ payload: { userId, date } }: SendUserReadReceipt) => {
            this.directlineService.sendReadReceipt(userId, date);
            return null;
        })
    )

    constructor (private actions$: Actions,
        private http: HttpClient,
        private directlineService: DirectlineService) { }
}
