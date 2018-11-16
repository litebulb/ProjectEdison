import { combineLatest, Observable, Subscription } from 'rxjs';
import { filter, first } from 'rxjs/operators';

import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Actions, ofType } from '@ngrx/effects';
import { select, Store } from '@ngrx/store';

import { environment } from '../../../../../environments/environment';
import { DirectlineService } from '../../../../core/services/directline/directline.service';
import { MessageModel } from '../../../../core/services/directline/models/activity-model';
import { AppState } from '../../../../reducers';
import { AppActionTypes, SetPageData } from '../../../../reducers/app/app.actions';
import {
    AddChat, ChatActionTypes, GetChatAuthToken, ToggleAllUsersChatWindow, ToggleUserChatWindow
} from '../../../../reducers/chat/chat.actions';
import { chatAuthSelector } from '../../../../reducers/chat/chat.selectors';
import { GetDevices } from '../../../../reducers/device/device.actions';
import { Device } from '../../../../reducers/device/device.model';
import { devicesFilteredSelector } from '../../../../reducers/device/device.selectors';
import { EventActionTypes, GetEvents, ShowEvents } from '../../../../reducers/event/event.actions';
import { Event, EventType } from '../../../../reducers/event/event.model';
import { eventsSelector } from '../../../../reducers/event/event.selectors';
import { GetResponses } from '../../../../reducers/response/response.actions';
import { Response, ResponseState } from '../../../../reducers/response/response.model';
import {
    responsesExist, responsesSelector
} from '../../../../reducers/response/response.selectors';
import { MapComponent } from '../../../map/components/map/map.component';
import { MapDefaults } from '../../../map/models/mapDefaults';
import { MapPin } from '../../../map/models/mapPin';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: [ './dashboard.component.scss' ]
})
export class DashboardComponent implements OnInit, OnDestroy {
    @ViewChild(MapComponent) private mapComponent: MapComponent

    showResponses$: Observable<boolean>
    combinedStream$: Subscription
    focusAllPinsSub$: Subscription
    showEventsSub$: Subscription
    showUserChat: boolean;
    showAllUserChat: boolean;

    defaultOptions: MapDefaults = {
        mapId: 'defaultMap',
        useHtmlLayer: true,
        zoom: environment.mapDefaults.zoom,
        showZoomControls: true,
    }

    pins: MapPin[] = []

    constructor (
        private store: Store<AppState>,
        private actions$: Actions,
        private directlineService: DirectlineService
    ) { }

    ngOnInit() {
        const responseStream = this.store.pipe(select(responsesSelector));
        const eventStream = this.store.pipe(select(eventsSelector));
        const deviceStream = this.store.pipe(select(devicesFilteredSelector));

        this.actions$
            .pipe(ofType(ChatActionTypes.ToggleAllUsersChatWindow))
            .subscribe(({ payload: { open } }: ToggleAllUsersChatWindow) => {
                this.showAllUserChat = open;
                this.showUserChat = false;
            })

        this.actions$
            .pipe(ofType(ChatActionTypes.ToggleUserChatWindow))
            .subscribe(({ payload: { open } }: ToggleUserChatWindow) => {
                this.showAllUserChat = false;
                this.showUserChat = open;
            })

        this.store
            .pipe(
                select(chatAuthSelector),
                filter(auth => auth.token !== null),
                first(auth => auth.token !== null))
            .subscribe(auth => {
                this.directlineService.connect(auth.token, auth.user).subscribe((chatMessage: MessageModel) => {
                    this.store.dispatch(new AddChat({ chat: chatMessage }))
                });
            });

        this.combinedStream$ = combineLatest(
            responseStream,
            eventStream,
            deviceStream
        ).subscribe(([ responses, events, devices ]) =>
            this.updatePins(devices, events, responses)
        )

        this.showEventsSub$ = this.actions$
            .pipe(ofType(EventActionTypes.ShowEvents))
            .subscribe(({ payload: { events } }: ShowEvents) => {
                if (this.pins.length > 0 && events && events.length > 0) {
                    this.mapComponent.focusEvents(events)
                }
            })

        this.focusAllPinsSub$ = this.actions$
            .ofType(AppActionTypes.FocusAllPins)
            .subscribe(() => {
                this.mapComponent.focusAllPins()
            })

        this.showResponses$ = this.store.pipe(select(responsesExist))

        this.store.dispatch(new GetDevices())
        this.store.dispatch(new GetEvents())
        this.store.dispatch(new GetResponses())
        this.store.dispatch(new SetPageData({ title: 'Right Now' }))
        if (environment.authorize) {
            this.store.dispatch(new GetChatAuthToken())
        }
    }

    ngOnDestroy() {
        this.combinedStream$.unsubscribe()
        this.focusAllPinsSub$.unsubscribe()
        this.showEventsSub$.unsubscribe()
    }

    getPinColor(event: Event, response: Response) {
        if (response) {
            if (response.responseState === ResponseState.Active) {
                return response.color // response is active
            } else {
                return 'green' // response has been resolved
            }
        } else {
            if (event.closureDate === null) {
                return 'blue'
            } else {
                return 'grey'
            }
        }
    }

    updatePins(devices: Device[], events: Event[], responses: Response[]) {
        const devicePins: MapPin[] = devices.map(device => {
            const pin: MapPin = { ...device }
            const recentEvent = this.getRecentEvent(events, device.deviceId)

            if (recentEvent) {
                const activeResponse = this.getActiveResponse(
                    responses,
                    recentEvent.eventClusterId
                )

                pin.event = recentEvent
                pin.color = this.getPinColor(recentEvent, activeResponse)
            }

            return pin
        })

        const eventPins: MapPin[] = events
            .filter(e => e.eventType === EventType.Message &&
                e.device.geolocation)
            .reduce((acc, event) => {
                const existingEvent = acc.find(e => e.device.deviceId === event.device.deviceId);
                if (!existingEvent) { acc.push(event); }
                else if (existingEvent.startDate < event.startDate) {
                    acc = acc.filter(e => e.device.deviceId === event.device.deviceId);
                    acc.push(event);
                }

                return acc;
            }, [] as Event[])
            .map(event => ({
                ...event.device,
                event: event,
                color: this.getPinColor(event, this.getActiveResponse(responses, event.eventClusterId))
            }))

        const actionPlanPins: MapPin[] = responses
            .filter(resp => resp.primaryEventClusterId === null &&
                resp.geolocation)
            .map(resp => ({
                deviceId: resp.responseId,
                deviceType: 'ActionPlan',
                geolocation: resp.geolocation,
                online: null,
                name: '',
                location1: '',
                location2: '',
                location3: '',
            }))

        this.pins = [ ...devicePins, ...eventPins, ...actionPlanPins ];
    }

    getActiveResponse(responses: Response[], eventClusterId: string) {
        return responses.find(
            response =>
                response.primaryEventClusterId === eventClusterId ||
                response.eventClusterIds.some(eci => eci === eventClusterId)
        )
    }

    getRecentEvent(events: Event[], deviceId) {
        if (events.length <= 0) {
            return null
        }

        return events
            .filter(event => event.device.deviceId === deviceId)
            .sort((a, b) => b.startDate - a.startDate)
            .slice(0, 1)[ 0 ]
    }
}
