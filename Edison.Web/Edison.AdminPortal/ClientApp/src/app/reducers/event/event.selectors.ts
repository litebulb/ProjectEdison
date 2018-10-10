import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AppState } from '..';
import { State, selectAll } from './event.reducer';

export const eventStateSelector = createFeatureSelector<AppState, State>('event');

export const eventsSelector = createSelector(eventStateSelector, selectAll);

export const activeEventSelector = createSelector(eventStateSelector, state => state.activeEvent);

export const activeEventsSelector = createSelector(eventsSelector, state => state.filter(event => event.closureDate === null));

export const showEventsSelector = createSelector(eventStateSelector, state => state.showEvents);
