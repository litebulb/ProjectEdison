import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AppState } from '..';
import { State, selectAll } from './response.reducer';
import { ResponseState } from './response.model';

export const responseStateSelector = createFeatureSelector<AppState, State>(
  'response'
);

export const responsesSelector = createSelector(
  responseStateSelector,
  selectAll
);

export const activeResponsesSelector = createSelector(
  responsesSelector,
  responses =>
    responses.filter(
      response => response.responseState === ResponseState.Active
    )
);

export const responsesExist = createSelector(
  responsesSelector,
  state => state.filter(response => response.responseState === ResponseState.Active).length > 0
);

export const activeResponseSelector = createSelector(
  responseStateSelector,
  state => state.activeResponse
);
