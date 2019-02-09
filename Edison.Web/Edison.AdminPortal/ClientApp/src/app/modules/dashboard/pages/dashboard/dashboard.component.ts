import { combineLatest, Observable, Subscription } from 'rxjs';
import { filter, first } from 'rxjs/operators';

import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Actions, ofType } from '@ngrx/effects';
import { select, Store } from '@ngrx/store';

import { environment } from '../../../../../environments/environment';
import { getEventColor } from '../../../../core/colorRank';
import {
  DirectLineCommand,
  DirectlineService,
} from '../../../../core/services/directline/directline.service';
import { MessageModel } from '../../../../core/services/directline/models/activity-model';
import { AppState } from '../../../../reducers';
import {
  GetActionPlan,
  GetActionPlans,
  SelectActionPlan,
} from '../../../../reducers/action-plan/action-plan.actions';
import {
  ActionPlan,
  AddEditAction,
} from '../../../../reducers/action-plan/action-plan.model';
import {
  actionPlansSelector,
  selectedActionPlanSelector,
} from '../../../../reducers/action-plan/action-plan.selectors';
import {
  AppActionTypes,
  AppPage,
  SetPageData,
  ToggleOverlay,
} from '../../../../reducers/app/app.actions';
import {
  AddChat,
  ChatActionTypes,
  EndConversation,
  GetChatAuthToken,
  SendNewMessage,
  SendUserReadReceipt,
  ToggleAllUsersChatWindow,
  ToggleUserChatWindow,
  UpdateUserReadReceipt,
} from '../../../../reducers/chat/chat.actions';
import { Message } from '../../../../reducers/chat/chat.model';
import {
  chatActiveMessagesSelector,
  chatActiveUsersCountSelector,
  chatAllMessagesSelector,
  chatTokenSelector,
} from '../../../../reducers/chat/chat.selectors';
import {
  DeviceActionTypes,
  FocusDevices,
  GetDevices,
} from '../../../../reducers/device/device.actions';
import { Device } from '../../../../reducers/device/device.model';
import { devicesFilteredSelector } from '../../../../reducers/device/device.selectors';
import {
  EventActionTypes,
  GetEvents,
  SelectActiveEvent,
  ShowEvents,
  UpdateEvent,
  ShowEventInEventBar,
} from '../../../../reducers/event/event.actions';
import {
  Event,
  EventInstance,
  EventType,
} from '../../../../reducers/event/event.model';
import {
  activeMobileEventsSelector,
  eventsSelector,
} from '../../../../reducers/event/event.selectors';
import {
  CloseResponse,
  GetResponse,
  GetResponses,
  PostNewResponse,
  ResponseActionTypes,
  RetryResponseActions,
  ShowActivateResponse,
  UpdateResponseActions,
  AddLocationToActiveResponse,
  UpdateResponse,
  ShowManageResponse,
  SelectActiveResponse,
  ShowSelectingLocation,
} from '../../../../reducers/response/response.actions';
import {
  Response,
  ResponseState,
} from '../../../../reducers/response/response.model';
import {
  activeResponsesSelector,
  responsesExist,
  responsesSelector,
  activeResponseSelector,
} from '../../../../reducers/response/response.selectors';
import { MapComponent } from '../../../map/components/map/map.component';
import { MapDefaults } from '../../../map/models/mapDefaults';
import { MapPin } from '../../../map/models/mapPin';
import { ActivateResponseComponent } from '../../components/activate-response/activate-response.component';
import { GeoLocation } from 'src/app/core/models/geoLocation';
import { ActiveResponsesComponent } from '../../components/active-responses/active-responses.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit, OnDestroy {
  @ViewChild(MapComponent) private _mapComponent: MapComponent;
  @ViewChild(ActivateResponseComponent)
  private _activateResponseComponent: ActivateResponseComponent;
  @ViewChild(ActiveResponsesComponent)
  private _activeResponsesComponent: ActiveResponsesComponent;

  private combinedStream$: Subscription;
  private focusAllPinsSub$: Subscription;
  private showEventsSub$: Subscription;
  private focusDevicesSub$: Subscription;
  private setPageDataSub$: Subscription;
  private messagesSub$: Subscription;
  private activeMobileEvents$: Subscription;
  private actionPlans$: Subscription;

  activeResponse$: Observable<Response>;
  activeResponses$: Observable<Response[]>;
  responses$: Observable<Response[]>;
  selectedActionPlan$: Observable<ActionPlan>;
  showResponses$: Observable<boolean>;
  messages$: Observable<Message[]>;
  userCount$: Observable<number>;
  showOverlay$: Observable<boolean>;

  actionPlans: ActionPlan[];
  showUserChat: boolean;
  showAllUserChat: boolean;
  activeMessages: Message[];
  useColumnLayout: boolean = false;
  showResponseButtons: boolean = true;
  userId: string;
  userName: string;
  activeEvent: Event;
  activeEventInstance: EventInstance;

  defaultOptions: MapDefaults = {
    mapId: 'defaultMap',
    useHtmlLayer: true,
    zoom: environment.mapDefaults.zoom,
    showZoomControls: true,
  };

  pins: MapPin[] = [];

  constructor(
    private store: Store<AppState>,
    private actions$: Actions,
    private directlineService: DirectlineService
  ) {}

  ngOnInit() {
    this._getPageData();
    this._initDirectLine();
    this._initMapData();
    this._initData();
    this._initMessageData();
    this._initMobileEventsData();
    this._initObservables();
    this._initWatchActions();
  }

  ngOnDestroy() {
    this.combinedStream$.unsubscribe();
    this.focusAllPinsSub$.unsubscribe();
    this.showEventsSub$.unsubscribe();
    this.focusDevicesSub$.unsubscribe();
    this.setPageDataSub$.unsubscribe();
    this.messagesSub$.unsubscribe();
    this.activeMobileEvents$.unsubscribe();
    this.actionPlans$.unsubscribe();
  }

  toggleOverlay(state: boolean) {
    this._mapComponent.toggleOverlay(state);
  }

  deactivateResponse(responseId: string) {
    this.store.dispatch(
      new CloseResponse({ responseId, state: ResponseState.Inactive })
    );
  }

  getFullResponse(responseId: string) {
    this.store.dispatch(new GetResponse({ responseId }));
  }

  addLocationToResponse(payload: { location: any; responseId: string }) {
    this.store.dispatch(new AddLocationToActiveResponse(payload));
  }

  updateResponseLocation(payload: {
    geolocation: GeoLocation;
    responseId: string;
  }) {
    this.store.dispatch(
      new UpdateResponse({
        response: {
          id: payload.responseId,
          changes: {
            geolocation: payload.geolocation,
          },
        },
      })
    );
  }

  selectActiveResponse(response: Response) {
    this.store.dispatch(new SelectActiveResponse({ response }));
  }

  handleEventClick(event: Event) {
    this.store.dispatch(new ShowEventInEventBar({ event }));
  }

  protected clearActiveResponse() {
    this.store.dispatch(new SelectActionPlan({ actionPlan: null }));
    this.store.dispatch(new SelectActiveEvent({ event: null }));

    this.toggleOverlay(false);
    this.activeEvent = null;
  }

  protected retryResponseActions(responseId: string) {
    this.store.dispatch(new RetryResponseActions({ responseId }));
  }

  protected updateResponseActions(payload: {
    response: Response;
    actions: AddEditAction[];
    isCloseAction: boolean;
  }) {
    this.store.dispatch(new UpdateResponseActions(payload));
  }

  protected postNewResponse({
    event,
    actionPlan,
  }: {
    event: Event;
    actionPlan: ActionPlan;
  }) {
    this.store.dispatch(
      new PostNewResponse({ event: this.activeEvent, actionPlan })
    );
    this.activeEvent = null;
  }

  protected activateResponse() {
    if (this.activeEvent && this.activeEventInstance) {
      this.store.dispatch(
        new ShowActivateResponse({
          event: this.activeEvent,
          actionPlanId: this.activeEventInstance.metadata.reportType,
        })
      );
    } else {
      this.store.dispatch(new ShowActivateResponse({ event: null }));
    }
  }

  protected sendNewMessage(messageContent: {
    message: string;
    userId: string;
  }) {
    this.store.dispatch(new SendNewMessage(messageContent));
  }

  protected closeAllUserChat() {
    this.store.dispatch(new ToggleAllUsersChatWindow({ open: false }));
  }

  protected closeUserChat() {
    this.store.dispatch(new ToggleUserChatWindow({ open: false }));
  }

  protected endUserChat() {
    this.store.dispatch(new EndConversation({ userId: this.userId }));
  }

  protected updateEventAddresses(events: Event[]) {
    events
      .filter(
        event =>
          event.device && event.device.geolocation && !event.device.location1
      )
      .forEach(event => {
        const { longitude, latitude } = event.device.geolocation;
        this._mapComponent.getAddressByLocation(longitude, latitude, addr => {
          this.updateEventLocation(event, addr);
        });
      });
  }

  protected getPinColor(event: Event, response: Response) {
    if (response) {
      if (response.responseState === ResponseState.Active) {
        return response.color; // response is active
      } else {
        const expired =
          new Date().getTime() > new Date(event.endDate).getTime();
        if (expired) {
          return 'grey';
        }

        return 'green'; // response has been resolved
      }
    } else {
      if (event.endDate === null) {
        return 'blue';
      } else {
        return 'grey';
      }
    }
  }

  protected updatePins(
    devices: Device[],
    events: Event[],
    responses: Response[]
  ) {
    // Device pins
    const devicePins: MapPin[] = devices.map(device => {
      const pin: MapPin = { ...device };
      const recentEvent = this.getRecentEvent(events, device.deviceId);

      if (recentEvent) {
        const activeResponse = this.getActiveResponse(
          responses,
          recentEvent.eventClusterId
        );

        pin.event = recentEvent;
        pin.color = getEventColor(recentEvent, activeResponse);
      }

      return pin;
    });

    // Mobile device pins
    const eventPins: MapPin[] = events
      .filter(e => e.eventType === EventType.Message && e.device.geolocation)
      .reduce(
        (acc, event) => {
          const existingEvent = acc.find(
            e => e.device.deviceId === event.device.deviceId
          );
          if (!existingEvent) {
            acc.push(event);
          } else if (existingEvent.startDate < event.startDate) {
            acc = acc.filter(e => e.device.deviceId === event.device.deviceId);
            acc.push(event);
          }

          return acc;
        },
        [] as Event[]
      )
      .map(event => {
        return {
          ...event.device,
          event: event,
          color: getEventColor(
            event,
            this.getActiveResponse(responses, event.eventClusterId)
          ),
        };
      });

    // Responses without an event
    const actionPlanPins: MapPin[] = responses
      .filter(resp => resp.primaryEventClusterId === null && resp.geolocation)
      .map(resp => ({
        deviceId: resp.responseId,
        deviceType: 'ActionPlan',
        geolocation: resp.geolocation,
        online: null,
        name: '',
        location1: '',
        location2: '',
        location3: '',
        icon: resp.icon,
        color: resp.color,
      }));

    this.pins = [...devicePins, ...eventPins, ...actionPlanPins];
  }

  protected updateEventLocation(event: Event, location: string) {
    const action = new UpdateEvent({
      event: {
        id: event.eventClusterId,
        changes: {
          ...event,
          device: {
            ...event.device,
            location1: location,
          },
        },
      },
    });
    this.store.dispatch(action);
  }

  protected getActiveResponse(responses: Response[], eventClusterId: string) {
    return responses.find(
      response =>
        response.primaryEventClusterId === eventClusterId ||
        response.eventClusterIds.some(eci => eci === eventClusterId)
    );
  }

  protected getRecentEvent(events: Event[], deviceId) {
    if (events.length <= 0) {
      return null;
    }

    return events
      .filter(event => event.device.deviceId === deviceId)
      .sort(
        (a, b) =>
          new Date(b.startDate).getTime() - new Date(a.startDate).getTime()
      )
      .slice(0, 1)[0];
  }

  protected showActiveEvent() {
    this.store.dispatch(new ShowEvents({ events: [this.activeEvent] }));
  }

  protected getActionPlan(actionPlanId: string) {
    this.store.dispatch(new GetActionPlan({ actionPlanId }));
  }

  protected selectActionPlan(actionPlan: ActionPlan) {
    this.store.dispatch(new SelectActionPlan({ actionPlan }));
  }

  private _initMobileEventsData() {
    this.activeMobileEvents$ = this.store
      .pipe(select(activeMobileEventsSelector))
      .subscribe(events => {
        const foundEvent = events.find(event =>
          event.events.some(ee => ee.metadata.userId === this.userId)
        );
        if (foundEvent) {
          this.activeEvent = foundEvent;
        }
        this._selectActiveEventInstance();
      });
  }

  private _selectActiveEventInstance() {
    if (this.activeEvent) {
      this.activeEventInstance = this.activeEvent.events.sort(
        (a, b) => new Date(b.date).getTime() - new Date(a.date).getTime()
      )[0];
    }
  }

  private _initMessageData() {
    this.messagesSub$ = this.store
      .pipe(select(chatActiveMessagesSelector))
      .subscribe(({ userMessages, userId, name }) => {
        const userChanged = this.userId !== userId;
        const messagesChanged =
          !this.activeMessages ||
          this.activeMessages.length !== userMessages.length ||
          userChanged;
        const messagesAdded =
          this.userId === userId &&
          userMessages.length > this.activeMessages.length;

        if (messagesAdded) {
          this.store.dispatch(
            new SendUserReadReceipt({ userId: this.userId, date: new Date() })
          );
        }

        if (messagesChanged) {
          this.activeMessages = userMessages;
        }

        if (userChanged) {
          this.userId = userId;
          this.userName = name;
        }
      });
  }

  private _initData() {
    this.actionPlans$ = this.store
      .pipe(select(actionPlansSelector))
      .subscribe(actionPlans => (this.actionPlans = actionPlans));
  }

  private _getPageData() {
    this.store.dispatch(new GetActionPlans());
    this.store.dispatch(new GetDevices());
    this.store.dispatch(new GetEvents());
    this.store.dispatch(new GetResponses());
    this.store.dispatch(
      new SetPageData({
        title: AppPage.RightNow,
        showDownArrow: true,
        showReloadButton: true,
      })
    );
    if (environment.authorize) {
      this.store.dispatch(new GetChatAuthToken());
    }
  }

  private _initWatchActions() {
    this.actions$
      .pipe(ofType(ChatActionTypes.ToggleAllUsersChatWindow))
      .subscribe(({ payload: { open } }: ToggleAllUsersChatWindow) => {
        this.showAllUserChat = open;
        this.showUserChat = false;
      });

    this.actions$
      .pipe(ofType(ChatActionTypes.ToggleUserChatWindow))
      .subscribe(({ payload: { open } }: ToggleUserChatWindow) => {
        this.showAllUserChat = false;
        this.showUserChat = open;
      });

    this.showEventsSub$ = this.actions$
      .pipe(ofType(EventActionTypes.ShowEvents))
      .subscribe(({ payload: { events } }: ShowEvents) => {
        if (this.pins.length > 0 && events && events.length > 0) {
          this._mapComponent.focusEvents(events);
        }
      });

    this.focusAllPinsSub$ = this.actions$
      .ofType(AppActionTypes.FocusAllPins)
      .subscribe(() => {
        this._mapComponent.focusAllPins();
      });

    this.focusDevicesSub$ = this.actions$
      .ofType(DeviceActionTypes.FocusDevices)
      .subscribe(({ payload: { devices } }: FocusDevices) => {
        this._mapComponent.focusDevices(devices);
      });

    this.setPageDataSub$ = this.actions$
      .ofType(AppActionTypes.UpdatePageData)
      .subscribe(({ payload: { title } }: SetPageData) => {
        setTimeout(() => {
          this.useColumnLayout = title === AppPage.Devices;
          this.showResponseButtons = title !== AppPage.Devices;
        });
      });

    this.actions$
      .pipe(ofType(ResponseActionTypes.ShowActivateResponse))
      .subscribe(
        ({ payload: { event, actionPlanId } }: ShowActivateResponse) => {
          const actionPlan = this.actionPlans.find(
            ap => ap.actionPlanId === actionPlanId
          );
          this.activeEvent = event;
          if (actionPlan) {
            this.toggleOverlay(true);
            this._selectActiveEventInstance();
            if (!actionPlan.openActions) {
              this.store.dispatch(new GetActionPlan({ actionPlanId }));
            }
          }
          this._activateResponseComponent.toggleActive();
        }
      );

    this.actions$
      .pipe(ofType(AppActionTypes.ToggleOverlay))
      .subscribe(({ payload: { state } }: ToggleOverlay) =>
        this.toggleOverlay(state)
      );

    this.actions$
      .pipe(ofType(ResponseActionTypes.ShowManageResponse))
      .subscribe(({ payload: { showManageResponse } }: ShowManageResponse) =>
        this._activeResponsesComponent.toggle(showManageResponse)
      );

    this.actions$
      .pipe(ofType(ResponseActionTypes.ShowSelectingLocation))
      .subscribe((action: ShowSelectingLocation) => {
        this._mapComponent.toggleSelectResponseLocation(
          action.payload.showSelectingLocation
        );
      });
  }

  private _initMapData() {
    const responseStream = this.store.pipe(select(responsesSelector));
    const eventStream = this.store.pipe(select(eventsSelector));
    const deviceStream = this.store.pipe(select(devicesFilteredSelector));

    this.combinedStream$ = combineLatest(
      responseStream,
      eventStream,
      deviceStream
    ).subscribe(([responses, events, devices]) => {
      this.updatePins(devices, events, responses);
      this.updateEventAddresses(events);
    });
  }

  private _initObservables() {
    this.messages$ = this.store.pipe(select(chatAllMessagesSelector));
    this.userCount$ = this.store.pipe(select(chatActiveUsersCountSelector));
    this.showResponses$ = this.store.pipe(select(responsesExist));
    this.selectedActionPlan$ = this.store.pipe(
      select(selectedActionPlanSelector)
    );
    this.activeResponses$ = this.store.pipe(select(activeResponsesSelector));
    this.responses$ = this.store.pipe(select(responsesSelector));
    this.activeResponse$ = this.store.pipe(select(activeResponseSelector));
  }

  private _initDirectLine() {
    this.store
      .pipe(
        select(chatTokenSelector),
        filter(auth => auth.token !== null),
        first(auth => auth.token !== null)
      )
      .subscribe(auth => {
        this.directlineService
          .connect(auth.token, auth.user)
          .subscribe((message: MessageModel) => {
            switch (message.channelData.baseCommand) {
              case DirectLineCommand.SendMessage:
                this.store.dispatch(new AddChat({ chat: message }));
                break;
              case DirectLineCommand.ReadUserMessages:
                if (message.channelData.data.userId) {
                  this.store.dispatch(
                    new UpdateUserReadReceipt({
                      userId: message.channelData.data.userId,
                      date: new Date(message.channelData.data.date),
                    })
                  );
                }
                break;
            }
          });
      });
  }
}
