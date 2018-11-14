import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { Observable, of } from 'rxjs';
import { Action } from '@ngrx/store';
import { mergeMap, catchError, map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { ChatActionTypes, GetChatAuthTokenSuccess, GetChatAuthTokenError, SendNewMessage, EndConversation, ToggleAllUsersChatWindow, SelectActiveUser, ToggleUserChatWindow } from '../reducers/chat/chat.actions';
import { DirectlineService } from '../core/services/directline/directline.service';

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

    @Effect({ dispatch: false })
    sendNewMessage$: Observable<void> = this.actions$.pipe(
        ofType(ChatActionTypes.SendNewMessage),
        map(({ payload: { message, userId } }: SendNewMessage) => {
            this.directlineService.sendMessage(message, userId);
            return null;
        })
    )

    @Effect()
    openChatWindow$: Observable<Action> = this.actions$.pipe(
        ofType(ChatActionTypes.ToggleAllUsersChatWindow, ChatActionTypes.ToggleUserChatWindow),
        map(({ payload }: ToggleUserChatWindow) => new SelectActiveUser(payload))
    )

    constructor (private actions$: Actions,
        private http: HttpClient,
        private directlineService: DirectlineService) { }
}
