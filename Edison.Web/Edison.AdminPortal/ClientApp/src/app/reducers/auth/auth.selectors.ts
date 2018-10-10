import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AppState } from '..';
import { State } from './auth.reducer';

export const authStateSelector = createFeatureSelector<AppState, State>('auth');

export const authTokenSelector = createSelector(authStateSelector, state => state.token);

export const authUserSelector = createSelector(authStateSelector, state => state.user);
