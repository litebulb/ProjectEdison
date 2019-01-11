import { Subscription } from 'rxjs';

import { Injectable, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@aspnet/signalr';
import { select, Store } from '@ngrx/store';

import { environment } from '../../../environments/environment';
import { AppState } from '../../reducers';
import { authTokenSelector } from '../../reducers/auth/auth.selectors';
import {
    SignalRDeleteDevice, SignalRNewDevice, SignalRUpdateDevice
} from '../../reducers/device/device.actions';
import {
    SignalRCloseEvent, SignalRNewEvent, SignalRUpdateEvent
} from '../../reducers/event/event.actions';
import {
    SignalRCloseResponse, SignalRNewResponse, SignalRUpdateResponse, SignalRUpdateResponseAction
} from '../../reducers/response/response.actions';
import { SignalRModels } from '../models/signalRModels';
import { SignalRTypes } from '../models/signalRTypes';

@Injectable({
    providedIn: 'root',
})
export class SignalRService implements OnDestroy {
    private connection: HubConnection
    private token: string
    private authTokenSub$: Subscription

    constructor (private store: Store<AppState>) {
        
    }

    ngOnDestroy() {
        this.authTokenSub$.unsubscribe()
    }

    init = () => {
        if (environment.mockData) { return; }
        this.authTokenSub$ = this.store
            .pipe(select(authTokenSelector))
            .subscribe(token => {
                if (token) {
                    this.token = token
                    if (!environment.mockData) {
                        this.setupSignalR()
                    }
                }
            })
    }

    private setupSignalR() {
        this.buildConnection()
        this.setupHandlers()
        this.startConnection()
    }

    private buildConnection = () => {
        const accessToken = this.token

        this.connection = new HubConnectionBuilder()
            .withUrl(`${environment.baseUrl}${environment.signalRUrl}`, {
                // transport: HttpTransportType.WebSockets,
                accessTokenFactory: () => accessToken
            })
            .configureLogging(LogLevel.Information)
            .build()
    }

    private startConnection = () => {
        this.connection.start().catch(err => console.log(err))
    }

    private setupHandlers = () => {
        this.connection.on(
            SignalRTypes.Channel.UpdateAction,
            (message: SignalRModels.ActionMessage) => {
                switch (message.updateType) {
                    case SignalRTypes.Action.ResponseActionCallback:
                        this.store.dispatch(new SignalRUpdateResponseAction({ message }));
                        break;
                }
            }
        )

        this.connection.on(
            SignalRTypes.Channel.Event,
            (message: SignalRModels.EventMessage) => {
                const { eventCluster } = message
                switch (message.updateType) {
                    case SignalRTypes.Event.New:
                        this.store.dispatch(new SignalRNewEvent({ event: eventCluster }))
                        break
                    case SignalRTypes.Event.Update:
                        this.store.dispatch(
                            new SignalRUpdateEvent({
                                event: {
                                    id: eventCluster.eventClusterId,
                                    changes: eventCluster,
                                },
                            })
                        )
                        break
                    case SignalRTypes.Event.Close:
                        this.store.dispatch(
                            new SignalRCloseEvent({
                                event: {
                                    id: eventCluster.eventClusterId,
                                    changes: eventCluster,
                                },
                            })
                        )
                        break
                }
            }
        )

        this.connection.on(
            SignalRTypes.Channel.Device,
            (message: SignalRModels.DeviceMessage) => {
                const { device, deviceId } = message
                switch (message.updateType) {
                    case SignalRTypes.Device.New:
                        this.store.dispatch(new SignalRNewDevice({ device }))
                        break
                    case SignalRTypes.Device.Update:
                        this.store.dispatch(
                            new SignalRUpdateDevice({
                                device: { id: deviceId, changes: device },
                            })
                        )
                        break
                    case SignalRTypes.Device.Delete:
                        this.store.dispatch(new SignalRDeleteDevice({ id: deviceId }))
                        break
                }
            }
        )

        this.connection.on(
            SignalRTypes.Channel.Response,
            (message: SignalRModels.ResponseMessage) => {
                const { response, responseId } = message
                switch (message.updateType) {
                    case SignalRTypes.Response.New:
                        this.store.dispatch(new SignalRNewResponse({ response }))
                        break
                    case SignalRTypes.Response.UpdateResponseActions:
                        // console.log(message);
                        break;
                    case SignalRTypes.Response.Update:
                        this.store.dispatch(
                            new SignalRUpdateResponse({
                                response: { id: responseId, changes: response },
                            })
                        )
                        break
                    case SignalRTypes.Response.Close:
                        this.store.dispatch(
                            new SignalRCloseResponse({
                                response: { id: responseId, changes: response },
                            })
                        )
                        break
                }
            }
        )
    }
}
