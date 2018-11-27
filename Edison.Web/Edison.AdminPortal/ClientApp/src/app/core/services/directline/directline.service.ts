import { interval, Observable, Subscriber, Subscription, throwError } from 'rxjs';
import { catchError, take } from 'rxjs/operators';

import { HttpBackend, HttpClient } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';

import { ActivityModel, MessageModel } from './models/activity-model';
import { ActivitiesResponseModel } from './models/response-models/activities-response.model';
import { ReconnectResponseModel } from './models/response-models/reconnect-response.model';
import { SendActivityResponseModel } from './models/response-models/send-activity-response.model';
import {
    StartConversationResponseModel
} from './models/response-models/start-conversation-response.model';
import { TokenResponseModel } from './models/response-models/token-response.model';
import { UserModel } from './models/user-model';

export enum DirectLineCommand {
    SendMessage = 'SendMessage',
    EndConversation = 'EndConversation',
    GetTranscript = 'getTranscript',
    ReadUserMessages = 'ReadUserMessages',
}

@Injectable({
    providedIn: 'root'
})
export class DirectlineService implements OnDestroy {
    private apiUrl = 'https://directline.botframework.com/v3/directline';
    private apiUrls = {
        generateToken: '/tokens/generate',
        refreshToken: '/tokens/refresh',
        conversations: '/conversations/'
    }

    private socket: WebSocket;
    private user: UserModel;
    private token: string;
    private baseToken: TokenResponseModel;
    private conversationToken: StartConversationResponseModel;

    private generateTokenSub$: Subscription;
    private refreshTokenSub$: Subscription;

    private httpClient: HttpClient;

    constructor (handler: HttpBackend) {
        this.httpClient = new HttpClient(handler);
    }

    ngOnDestroy() {
        if (this.generateTokenSub$) { this.generateTokenSub$.unsubscribe() }
        if (this.refreshTokenSub$) { this.refreshTokenSub$.unsubscribe() }
    }

    connect(token: string, user: UserModel): Observable<MessageModel> {
        this.token = token;
        this.user = user;

        return Observable.create((subscriber: Subscriber<MessageModel>) => {
            this._startConversation(this.token).subscribe(response => {
                this.conversationToken = response;

                this._subscribeToMessages(response.streamUrl, subscriber)
                    .pipe(take(1))
                    .subscribe(() => this._getTranscript());
            });
        })
    }

    endConversation(userId: string) {
        this._sendCommand(DirectLineCommand.EndConversation, userId);
    }

    sendMessage(message: string, sendToUserId: string) {
        this._sendMessage(message, sendToUserId);
    }

    sendReadReceipt(userId: string, date: Date) {
        this._sendCommand(DirectLineCommand.ReadUserMessages, null, {
            userId,
            date
        })
    }

    private _getTranscript() {
        this._sendCommand(DirectLineCommand.GetTranscript);
    }

    private _subscribeToMessages(streamUrl: string, subscriber) {
        this.socket = new WebSocket(streamUrl);
        let sub: Subscription;

        this.socket.onmessage = message => {
            if (message.type === 'message' && message.data && message.data.length > 0) {
                const parsedMessage: ActivitiesResponseModel = JSON.parse(message.data);
                subscriber.next(parsedMessage.activities[ 0 ]);
            }
        };

        const onOpen$ = Observable.create(subscriber => {
            this.socket.onopen = open => {
                // Chrome is pretty bad at noticing when a WebSocket connection is broken.
                // If we periodically ping the server with empty messages, it helps Chrome
                // realize when connection breaks, and close the socket. We then throw an
                // error, and that give us the opportunity to attempt to reconnect.
                sub = interval(1000).subscribe(_ => this.socket.send(''));
                subscriber.next();
            }
        });

        this.socket.onclose = close => {
            if (sub) sub.unsubscribe();
            subscriber.error(close);
        };

        return onOpen$;
    }

    private _generateToken(secret: string) {
        this.generateTokenSub$ = this.httpClient
            .post<TokenResponseModel>(`${this.apiUrl}${this.apiUrls.generateToken}`, { user: this.user }, {
                headers: {
                    Authorization: `Bearer ${secret}`,
                }
            })
            .pipe(catchError(err => throwError(err)))
            .subscribe(response => {
                this.baseToken = response;
            });
    }

    private _refreshToken() {
        this.refreshTokenSub$ = this.httpClient
            .post<TokenResponseModel>(`${this.apiUrl}${this.apiUrls.refreshToken}`, null, {
                headers: {
                    Authorization: `Bearer ${this.conversationToken.token}`,
                }
            })
            .pipe(catchError(err => throwError(err)))
            .subscribe(response => this.baseToken = response);
    }

    private _startConversation(tokenOrSecret: string) {
        return this.httpClient
            .post<StartConversationResponseModel>(`${this.apiUrl}${this.apiUrls.conversations}`, null, {
                headers: {
                    Authorization: `Bearer ${tokenOrSecret}`,
                }
            })
            .pipe(catchError(err => throwError(err)))
    }

    private _reconnectConversation(conversationId: string, watermark?: string) {
        const url = `${this.apiUrl}${this.apiUrls.conversations}${conversationId}${watermark ? `?watermark=${watermark}` : ''}`;
        this.httpClient
            .get<ReconnectResponseModel>(url, {
                headers: {
                    Authorization: `Bearer ${this.baseToken.token}`,
                }
            })
            .pipe(catchError(err => throwError(err)))
            .subscribe(response => this.conversationToken = {
                ...this.conversationToken,
                ...response,
            });
    }

    private _getActivities(conversationId: string, watermark?: string) {
        const url = `${this.apiUrl}${this.apiUrls.conversations}${conversationId}/activities${watermark ? `?watermark=${watermark}` : ''}`;
        this.httpClient
            .get<ActivitiesResponseModel>(url, {
                headers: {
                    Authorization: `Bearer ${this.token}`
                }
            })
            .pipe(catchError(err => throwError(err)))
            .subscribe(response => {
            })
    }

    private _sendMessage(message: string, sendToUserId: string) {
        this._sendActivity(DirectLineCommand.SendMessage, message, sendToUserId);
    }

    private _sendCommand(command: DirectLineCommand, sendToUserId?: string, channelData?) {
        this._sendActivity(command, null, sendToUserId, channelData);
    }

    private _sendActivity(command: DirectLineCommand, message?: string, sendToUserId?: string, channelData?) {
        const { conversationId } = this.conversationToken;
        const activity: ActivityModel = {
            type: 'message',
            from: this.user,
            text: message,
            channelData: {
                baseCommand: command,
                data: {
                    from: this.user,
                    userId: sendToUserId,
                    ...channelData,
                }
            }
        }

        this.httpClient
            .post<SendActivityResponseModel>(`${this.apiUrl}${this.apiUrls.conversations}${conversationId}/activities`, activity, {
                headers: {
                    Authorization: `Bearer ${this.token}`
                }
            })
            .pipe(catchError(err => throwError(err)))
            .subscribe(response => {
            })
    }
}
