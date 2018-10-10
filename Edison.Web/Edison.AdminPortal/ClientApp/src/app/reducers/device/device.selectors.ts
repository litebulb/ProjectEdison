import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AppState } from '..';
import { State, selectAll } from './device.reducer';

export const deviceStateSelector = createFeatureSelector<AppState, State>('device');

export const devicesSelector = createSelector(deviceStateSelector, selectAll);
