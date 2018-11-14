import { createFeatureSelector, createSelector } from '@ngrx/store'
import { AppState } from '..'
import { State } from './app.reducer'

export const appStateSelector = createFeatureSelector<AppState, State>('app')

export const pageTitleSelector = createSelector(
  appStateSelector,
  state => state.pageTitle
)
