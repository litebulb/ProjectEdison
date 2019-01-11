import { createFeatureSelector, createSelector } from '@ngrx/store';

import { AppState } from '../';
import { ResponseState } from './response.model';
import { selectAll, State } from './response.reducer';

export const responseStateSelector = createFeatureSelector<AppState, State>(
    'response'
);

export const showSelectingLocationSelector = createSelector(responseStateSelector,
    state => state.showSelectingLocation);

export const showManageResponseSelector = createSelector(responseStateSelector,
    state => state.showManageResponse);

export const responsesSelector = createSelector(
    responseStateSelector,
    selectAll
);

export const activeResponsesSelector = createSelector(
    responsesSelector,
    responses => {
        const result = responses.filter(response => response.responseState === ResponseState.Active);
        if (result) { return result; }
        return [];
    }
);

export const responsesExist = createSelector(
    responsesSelector,
    state => state.length > 0
);

export const activeResponseSelector = createSelector(
    responseStateSelector,
    state => state.activeResponse
);
