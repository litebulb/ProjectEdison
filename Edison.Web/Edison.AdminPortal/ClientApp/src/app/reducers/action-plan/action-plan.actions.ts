import { Update } from '@ngrx/entity';
import { Action } from '@ngrx/store';

import { ActionPlan } from './action-plan.model';

export enum ActionPlanActionTypes {
    LoadActionPlans = '[ActionPlan] Load ActionPlans',
    AddActionPlan = '[ActionPlan] Add ActionPlan',
    UpsertActionPlan = '[ActionPlan] Upsert ActionPlan',
    AddActionPlans = '[ActionPlan] Add ActionPlans',
    UpsertActionPlans = '[ActionPlan] Upsert ActionPlans',
    UpdateActionPlan = '[ActionPlan] Update ActionPlan',
    UpdateActionPlans = '[ActionPlan] Update ActionPlans',
    DeleteActionPlan = '[ActionPlan] Delete ActionPlan',
    DeleteActionPlans = '[ActionPlan] Delete ActionPlans',
    ClearActionPlans = '[ActionPlan] Clear ActionPlans',
    SelectActionPlan = '[ActionPlan] Select Action Plan',
    GetActionPlans = '[ActionPlan] Get Action Plans',
    GetActionPlansError = '[ActionPlan] Get Action Plans Error',
    GetActionPlan = '[ActionPlan] Get Action Plan',
    GetActionPlanError = '[ActionPlan] Get Action Plan Error',
    PutActionPlan = '[ActionPlan] Put Action Plan',
    PutActionPlanSuccess = '[ActionPlan] Put Action Plan Success',
    PutActionPlanError = '[ActionPlan] Put Action Plan Error',
}

export class PutActionPlan implements Action {
    readonly type = ActionPlanActionTypes.PutActionPlan;

    constructor (public payload: { actionPlan: ActionPlan }) { }
}

export class PutActionPlanSuccess implements Action {
    readonly type = ActionPlanActionTypes.PutActionPlanSuccess;

    constructor (public payload: { actionPlan: ActionPlan }) { }
}

export class PutActionPlanError implements Action {
    readonly type = ActionPlanActionTypes.PutActionPlanError;

    constructor (public payload: { actionPlan: ActionPlan }) { }
}

export class GetActionPlan implements Action {
    readonly type = ActionPlanActionTypes.GetActionPlan;

    constructor (public payload: { actionPlanId: string }) { }
}

export class GetActionPlanError implements Action {
    readonly type = ActionPlanActionTypes.GetActionPlanError;
}

export class GetActionPlans implements Action {
    readonly type = ActionPlanActionTypes.GetActionPlans;
}

export class GetActionPlansError implements Action {
    readonly type = ActionPlanActionTypes.GetActionPlansError;
}

export class SelectActionPlan implements Action {
    readonly type = ActionPlanActionTypes.SelectActionPlan;

    constructor (public payload: { actionPlan: ActionPlan }) { }
}

export class LoadActionPlans implements Action {
    readonly type = ActionPlanActionTypes.LoadActionPlans;

    constructor (public payload: { actionPlans: ActionPlan[] }) { }
}

export class AddActionPlan implements Action {
    readonly type = ActionPlanActionTypes.AddActionPlan;

    constructor (public payload: { actionPlan: ActionPlan }) { }
}

export class UpsertActionPlan implements Action {
    readonly type = ActionPlanActionTypes.UpsertActionPlan;

    constructor (public payload: { actionPlan: ActionPlan }) { }
}

export class AddActionPlans implements Action {
    readonly type = ActionPlanActionTypes.AddActionPlans;

    constructor (public payload: { actionPlans: ActionPlan[] }) { }
}

export class UpsertActionPlans implements Action {
    readonly type = ActionPlanActionTypes.UpsertActionPlans;

    constructor (public payload: { actionPlans: ActionPlan[] }) { }
}

export class UpdateActionPlan implements Action {
    readonly type = ActionPlanActionTypes.UpdateActionPlan;

    constructor (public payload: { actionPlan: Update<ActionPlan> }) { }
}

export class UpdateActionPlans implements Action {
    readonly type = ActionPlanActionTypes.UpdateActionPlans;

    constructor (public payload: { actionPlans: Update<ActionPlan>[] }) { }
}

export class DeleteActionPlan implements Action {
    readonly type = ActionPlanActionTypes.DeleteActionPlan;

    constructor (public payload: { id: string }) { }
}

export class DeleteActionPlans implements Action {
    readonly type = ActionPlanActionTypes.DeleteActionPlans;

    constructor (public payload: { ids: string[] }) { }
}

export class ClearActionPlans implements Action {
    readonly type = ActionPlanActionTypes.ClearActionPlans;
}

export type ActionPlanActions =
    LoadActionPlans
    | AddActionPlan
    | UpsertActionPlan
    | AddActionPlans
    | UpsertActionPlans
    | UpdateActionPlan
    | UpdateActionPlans
    | DeleteActionPlan
    | DeleteActionPlans
    | ClearActionPlans
    | SelectActionPlan
    | PutActionPlan
    | PutActionPlanSuccess
    | PutActionPlanError;
