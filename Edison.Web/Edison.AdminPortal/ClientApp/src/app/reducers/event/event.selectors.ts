import { createFeatureSelector, createSelector } from '@ngrx/store';

import { AppState } from '../';
import { EventType } from './event.model';
import { selectAll, State } from './event.reducer';

export const eventStateSelector = createFeatureSelector<AppState, State>('event');

export const eventsSelector = createSelector(eventStateSelector,
    state => selectAll(state).filter(event => !event.endDate ||
        (Math.abs(new Date().getTime() - event.endDate.getTime()) / 36e5) <= 12));

export const activeEventSelector = createSelector(eventStateSelector, state => state.activeEvent);

export const activeEventsSelector = createSelector(eventsSelector,
    state => state.filter(event => event.closureDate === null));

export const activeMobileEventsSelector = createSelector(activeEventsSelector,
    state => state.filter(event => event.eventType === EventType.Message))
