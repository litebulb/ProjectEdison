import {
  ActionReducer,
  ActionReducerMap,
  createFeatureSelector,
  createSelector,
  MetaReducer,
} from '@ngrx/store';
import { environment } from '../../environments/environment';
import * as fromRouterStore from '@ngrx/router-store';

export interface State {
  router: fromRouterStore.RouterReducerState;
}

export const reducers: ActionReducerMap<State> = {
  router: fromRouterStore.routerReducer,
};

export const metaReducers: MetaReducer<State>[] = !environment.production
  ? []
  : [];
