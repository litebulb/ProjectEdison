import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Event } from './event.model';
import { EventActions, EventActionTypes } from './event.actions';

export interface State extends EntityState<Event> {
    // additional entities state properties
    showEvents: Event[];
    activeEvent: Event;
}

export function sortByStartDate(a: Event, b: Event): number {
    return new Date(b.startDate).getTime() - new Date(a.startDate).getTime();
}

export const adapter: EntityAdapter<Event> = createEntityAdapter<Event>({
    selectId: (event: Event) => event.eventClusterId,
    sortComparer: sortByStartDate,
});

export const initialState: State = adapter.getInitialState({
    // additional entity state properties
    showEvents: [],
    activeEvent: null,
});

export function reducer(state = initialState, action: EventActions): State {
    switch (action.type) {
        case EventActionTypes.SignalRNewEvent:
        case EventActionTypes.AddEvent: {
            return adapter.addOne(action.payload.event, state);
        }

        case EventActionTypes.UpsertEvent: {
            return adapter.upsertOne(action.payload.event, state);
        }

        case EventActionTypes.AddEvents: {
            return adapter.addMany(action.payload.events, state);
        }

        case EventActionTypes.UpsertEvents: {
            return adapter.upsertMany(action.payload.events, state);
        }

        case EventActionTypes.UpdateEvent:
        case EventActionTypes.SignalRUpdateEvent:
        case EventActionTypes.SignalRCloseEvent: {
            return adapter.updateOne(action.payload.event, state);
        }

        case EventActionTypes.UpdateEvents: {
            return adapter.updateMany(action.payload.events, state);
        }

        case EventActionTypes.DeleteEvent: {
            return adapter.removeOne(action.payload.id, state);
        }

        case EventActionTypes.DeleteEvents: {
            return adapter.removeMany(action.payload.ids, state);
        }

        case EventActionTypes.LoadEvents: {
            return adapter.addAll(action.payload.events, state);
        }

        case EventActionTypes.ClearEvents: {
            return adapter.removeAll(state);
        }

        case EventActionTypes.ShowEvents: {
            return {
                ...state,
                showEvents: action.payload.events,
            };
        }

        case EventActionTypes.SelectActiveEvent: {
            return {
                ...state,
                activeEvent: action.payload.event,
            };
        }

        default: {
            return state;
        }
    }
}

export const {
    selectIds,
    selectEntities,
    selectAll,
    selectTotal,
} = adapter.getSelectors();
