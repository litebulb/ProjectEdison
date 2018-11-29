import { Update } from '@ngrx/entity';
import { Action } from '@ngrx/store';

import { Event } from './event.model';

export enum EventActionTypes {
    LoadEvents = '[Event] Load Events',
    AddEvent = '[Event] Add Event',
    UpsertEvent = '[Event] Upsert Event',
    AddEvents = '[Event] Add Events',
    UpsertEvents = '[Event] Upsert Events',
    UpdateEvent = '[Event] Update Event',
    UpdateEvents = '[Event] Update Events',
    DeleteEvent = '[Event] Delete Event',
    DeleteEvents = '[Event] Delete Events',
    ClearEvents = '[Event] Clear Events',
    GetEvents = '[Event] Get Events',
    GetEventsError = '[Event] Get Events Error',
    ShowEvents = '[Event] Show Events',
    SignalRNewEvent = '[Event] SignalR New Event',
    SignalRUpdateEvent = '[Event] SignalR Update Event',
    SignalRCloseEvent = '[Event] SignalR Close Event',
    SelectActiveEvent = '[Event] Select Active Event',
    ShowEventInEventBar = '[Event] Show Event In Event Bar'
}

export class ShowEventInEventBar implements Action {
    readonly type = EventActionTypes.ShowEventInEventBar;

    constructor (public payload: { event: Event }) { }
}

export class SelectActiveEvent implements Action {
    readonly type = EventActionTypes.SelectActiveEvent;

    constructor (public payload: { event: Event }) { }
}

export class SignalRNewEvent implements Action {
    readonly type = EventActionTypes.SignalRNewEvent;

    constructor (public payload: { event: Event }) { }
}

export class SignalRUpdateEvent implements Action {
    readonly type = EventActionTypes.SignalRUpdateEvent;

    constructor (public payload: { event: Update<Event> }) { }
}

export class SignalRCloseEvent implements Action {
    readonly type = EventActionTypes.SignalRCloseEvent;

    constructor (public payload: { event: Update<Event> }) { }
}

export class GetEvents implements Action {
    readonly type = EventActionTypes.GetEvents;
}

export class ShowEvents implements Action {
    readonly type = EventActionTypes.ShowEvents;

    constructor (public payload: { events: Event[] }) { }
}

export class GetEventsError implements Action {
    readonly type = EventActionTypes.GetEventsError;
}

export class LoadEvents implements Action {
    readonly type = EventActionTypes.LoadEvents;

    constructor (public payload: { events: Event[] }) { }
}

export class AddEvent implements Action {
    readonly type = EventActionTypes.AddEvent;

    constructor (public payload: { event: Event }) { }
}

export class UpsertEvent implements Action {
    readonly type = EventActionTypes.UpsertEvent;

    constructor (public payload: { event: Event }) { }
}

export class AddEvents implements Action {
    readonly type = EventActionTypes.AddEvents;

    constructor (public payload: { events: Event[] }) { }
}

export class UpsertEvents implements Action {
    readonly type = EventActionTypes.UpsertEvents;

    constructor (public payload: { events: Event[] }) { }
}

export class UpdateEvent implements Action {
    readonly type = EventActionTypes.UpdateEvent;

    constructor (public payload: { event: Update<Event> }) { }
}

export class UpdateEvents implements Action {
    readonly type = EventActionTypes.UpdateEvents;

    constructor (public payload: { events: Update<Event>[] }) { }
}

export class DeleteEvent implements Action {
    readonly type = EventActionTypes.DeleteEvent;

    constructor (public payload: { id: string }) { }
}

export class DeleteEvents implements Action {
    readonly type = EventActionTypes.DeleteEvents;

    constructor (public payload: { ids: string[] }) { }
}

export class ClearEvents implements Action {
    readonly type = EventActionTypes.ClearEvents;
}

export type EventActions =
    LoadEvents
    | AddEvent
    | UpsertEvent
    | AddEvents
    | UpsertEvents
    | UpdateEvent
    | UpdateEvents
    | DeleteEvent
    | DeleteEvents
    | ClearEvents
    | ShowEvents
    | SignalRNewEvent
    | SignalRUpdateEvent
    | SignalRCloseEvent
    | SelectActiveEvent;
