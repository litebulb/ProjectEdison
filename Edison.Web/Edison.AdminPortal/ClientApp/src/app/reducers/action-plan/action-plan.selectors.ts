import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AppState } from '..';
import { State, selectAll } from './action-plan.reducer';

export const actionPlanStateSelector = createFeatureSelector<AppState, State>('actionPlan');

export const actionPlansSelector = createSelector(actionPlanStateSelector, selectAll);

export const selectedActionPlanSelector = createSelector(actionPlanStateSelector, state => state.selectedActionPlan);
