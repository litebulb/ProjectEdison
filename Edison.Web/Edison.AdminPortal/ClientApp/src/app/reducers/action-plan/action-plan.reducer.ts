import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { ActionPlan } from './action-plan.model';
import { ActionPlanActions, ActionPlanActionTypes } from './action-plan.actions';

export interface State extends EntityState<ActionPlan> {
    // additional entities state properties
    selectedActionPlan: ActionPlan;
    selectingActionPlan: boolean;
}

export const adapter: EntityAdapter<ActionPlan> = createEntityAdapter<ActionPlan>({
    selectId: (actionPlan: ActionPlan) => actionPlan.actionPlanId,
    sortComparer: (a, b) => a.name.localeCompare(b.name),
});

export const initialState: State = adapter.getInitialState({
    // additional entity state properties
    selectedActionPlan: null,
    selectingActionPlan: false,
});

export function reducer(
    state = initialState,
    action: ActionPlanActions
): State {
    switch (action.type) {
        case ActionPlanActionTypes.AddActionPlan: {
            return adapter.addOne(action.payload.actionPlan, state);
        }

        case ActionPlanActionTypes.UpsertActionPlan: {
            return adapter.upsertOne(action.payload.actionPlan, state);
        }

        case ActionPlanActionTypes.AddActionPlans: {
            return adapter.addMany(action.payload.actionPlans, state);
        }

        case ActionPlanActionTypes.UpsertActionPlans: {
            return adapter.upsertMany(action.payload.actionPlans, state);
        }

        case ActionPlanActionTypes.UpdateActionPlan: {
            const isSelectedActionPlan = state.selectedActionPlan && state.selectedActionPlan.actionPlanId === action.payload.actionPlan.id;
            if (isSelectedActionPlan) {
                const newState = adapter.updateOne(action.payload.actionPlan, state);
                const updatedActionPlan = selectAll(newState).find(ap => ap.actionPlanId === action.payload.actionPlan.id);

                return {
                    ...adapter.updateOne(action.payload.actionPlan, state),
                    selectedActionPlan: updatedActionPlan,
                };
            }
            return adapter.updateOne(action.payload.actionPlan, state);
        }

        case ActionPlanActionTypes.UpdateActionPlans: {
            return adapter.updateMany(action.payload.actionPlans, state);
        }

        case ActionPlanActionTypes.DeleteActionPlan: {
            return adapter.removeOne(action.payload.id, state);
        }

        case ActionPlanActionTypes.DeleteActionPlans: {
            return adapter.removeMany(action.payload.ids, state);
        }

        case ActionPlanActionTypes.LoadActionPlans: {
            return adapter.addAll(action.payload.actionPlans, state);
        }

        case ActionPlanActionTypes.ClearActionPlans: {
            return adapter.removeAll(state);
        }

        case ActionPlanActionTypes.SelectActionPlan: {
            return {
                ...state,
                selectedActionPlan: action.payload.actionPlan,
            };
        }

        case ActionPlanActionTypes.SetSelectingActionPlan: {
            return {
                ...state,
                selectingActionPlan: action.payload.isSelecting,
            }
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
