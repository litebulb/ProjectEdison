import { Update } from '@ngrx/entity';
import { Action } from '@ngrx/store';

import { SignalRModels } from '../../core/models/signalRModels';
import { MapPosition } from '../../modules/map/models/mapPosition';
import { ActionPlan, AddEditAction } from '../action-plan/action-plan.model';
import { Event } from '../event/event.model';
import { Response, ResponseState } from './response.model';

export enum ResponseActionTypes {
    LoadResponses = '[Response] Load Responses',
    AddResponse = '[Response] Add Response',
    UpsertResponse = '[Response] Upsert Response',
    AddResponses = '[Response] Add Responses',
    UpsertResponses = '[Response] Upsert Responses',
    UpdateResponse = '[Response] Update Response',
    UpdateResponseAsync = '[Response] Update Response Async',
    UpdateResponses = '[Response] Update Responses',
    DeleteResponse = '[Response] Delete Response',
    DeleteResponses = '[Response] Delete Responses',
    ClearResponses = '[Response] Clear Responses',
    PostNewResponse = '[Response] Post New Response',
    PostNewResponseError = '[Response] Post New Response Error',
    PostNewResponseSuccess = '[Response] Post New Response Success',
    PutResponse = '[Response] Put Response',
    PutResponseError = '[Response] Put Response Error',
    GetResponses = '[Response] Get Responses',
    GetResponsesError = '[Response] Get Responses Error',
    SelectActiveResponse = '[Response] Select Active Response',
    SignalRNewResponse = '[Response] SignalR New Response',
    SignalRUpdateResponse = '[Response] SignalR Update Response',
    SignalRCloseResponse = '[Response] SignalR Close Response',
    GetResponse = '[Response] Get Response',
    GetResponseError = '[Response] Get Response Error',
    CloseResponse = '[Response] Close Response',
    CloseResponseError = '[Response] Close Response Error',
    ShowSelectingLocation = '[Response] Show Selecting Location',
    DontShowSelectingLocation = '[Response] Dont Show Selecting Location',
    AddLocationToActiveResponse = '[Response] Add Location To Response',
    AddLocationToActiveResponseError = '[Response] Add Location To Response Error',
    AddLocationToActiveResponseSuccess = '[Response] Add Location To Response Success',
    ShowManageResponse = '[Response] Show Manage Response',
    UpdateResponseActions = '[Response] Update Response Actions',
    UpdateResponseActionsSuccess = '[Response] Update Response Actions Success',
    UpdateResponseActionsError = '[Response] Update Response Actions Error',
    ShowActivateResponse = '[Response] Show Activate Response',
    SignalRUpdateResponseAction = '[Response] SignalR Update Response Action',
    ActivateResponseActionPlan = '[Response] Activate Response Action Plan',
    RetryResponseActions = '[Response] Retry Response Actions',
    RetryResponseActionsSuccess = '[Response] Retry Response Actions Success',
    RetryResponseActionsError = '[Response] Retry Response Actions Error',
    ResponseNonAction = '[Response] Response Non Action',
}

export enum LoadingType {
    All,
    Closed,
    Open,
}

export class ResponseNonAction implements Action {
    readonly type = ResponseActionTypes.ResponseNonAction;
}

export class RetryResponseActions implements Action {
    readonly type = ResponseActionTypes.RetryResponseActions;

    constructor (public payload: { responseId: string }) { }
}

export class RetryResponseActionsSuccess implements Action {
    readonly type = ResponseActionTypes.RetryResponseActionsSuccess;
}

export class RetryResponseActionsError implements Action {
    readonly type = ResponseActionTypes.RetryResponseActionsError;
}

export class ActivateResponseActionPlan implements Action {
    readonly type = ResponseActionTypes.ActivateResponseActionPlan;

    constructor (public payload: { response: Response }) { }
}

export class SignalRUpdateResponseAction implements Action {
    readonly type = ResponseActionTypes.SignalRUpdateResponseAction;

    constructor (public payload: { message: SignalRModels.ActionMessage }) { }
}

export class ShowActivateResponse implements Action {
    readonly type = ResponseActionTypes.ShowActivateResponse;

    constructor (public payload: { event?: Event, actionPlanId?: string }) { }
}

export class UpdateResponseActions implements Action {
    readonly type = ResponseActionTypes.UpdateResponseActions;

    constructor (public payload: { response: Response, actions: AddEditAction[], isCloseAction: boolean }) { }
}

export class UpdateResponseActionsSuccess implements Action {
    readonly type = ResponseActionTypes.UpdateResponseActionsSuccess;

    constructor (public payload: { response: Response }) { }
}

export class UpdateResponseActionsError implements Action {
    readonly type = ResponseActionTypes.UpdateResponseActionsError;
}

export class ShowManageResponse implements Action {
    readonly type = ResponseActionTypes.ShowManageResponse;

    constructor (public payload: { showManageResponse: boolean }) { }
}

export class AddLocationToActiveResponseError implements Action {
    readonly type = ResponseActionTypes.AddLocationToActiveResponseError;
}

export class AddLocationToActiveResponseSuccess implements Action {
    readonly type = ResponseActionTypes.AddLocationToActiveResponseSuccess;
}

export class AddLocationToActiveResponse implements Action {
    readonly type = ResponseActionTypes.AddLocationToActiveResponse;

    constructor (public payload: { location: MapPosition, responseId: string }) { }
}

export class ShowSelectingLocation implements Action {
    readonly type = ResponseActionTypes.ShowSelectingLocation;

    constructor (public payload: { showSelectingLocation: boolean, response?: Response }) { }
}

export class DontShowSelectingLocation implements Action {
    readonly type = ResponseActionTypes.DontShowSelectingLocation;
}

export class CloseResponse implements Action {
    readonly type = ResponseActionTypes.CloseResponse;

    constructor (public payload: { responseId: string, state: ResponseState }) { }
}

export class CloseResponseError implements Action {
    readonly type = ResponseActionTypes.CloseResponseError;
}

export class GetResponse implements Action {
    readonly type = ResponseActionTypes.GetResponse;

    constructor (public payload: { responseId: string }) { }
}

export class GetResponseError implements Action {
    readonly type = ResponseActionTypes.GetResponseError;
}

export class SignalRNewResponse implements Action {
    readonly type = ResponseActionTypes.SignalRNewResponse;

    constructor (public payload: { response: Response }) { }
}

export class SignalRUpdateResponse implements Action {
    readonly type = ResponseActionTypes.SignalRUpdateResponse;

    constructor (public payload: { response: Update<Response> }) { }
}

export class SignalRCloseResponse implements Action {
    readonly type = ResponseActionTypes.SignalRCloseResponse;

    constructor (public payload: { response: Update<Response> }) { }
}

export class SelectActiveResponse implements Action {
    readonly type = ResponseActionTypes.SelectActiveResponse;

    constructor (public payload: { response: Response }) { }
}

export class GetResponsesError implements Action {
    readonly type = ResponseActionTypes.GetResponsesError;
}

export class GetResponses implements Action {
    readonly type = ResponseActionTypes.GetResponses;
}

export class PutResponseError implements Action {
    readonly type = ResponseActionTypes.PutResponseError;
}

export class PutResponse implements Action {
    readonly type = ResponseActionTypes.PutResponse;

    constructor (public payload: { response: Response }) { }
}

export class PostNewResponseError implements Action {
    readonly type = ResponseActionTypes.PostNewResponseError;
}

export class PostNewResponseSuccess implements Action {
    readonly type = ResponseActionTypes.PostNewResponseSuccess;

    constructor (public payload: { response: Response }) { }
}

export class PostNewResponse implements Action {
    readonly type = ResponseActionTypes.PostNewResponse;

    constructor (public payload: { event?: Event, actionPlan: ActionPlan }) { }
}

export class LoadResponses implements Action {
    readonly type = ResponseActionTypes.LoadResponses;

    constructor (public payload: { responses: Response[] }) { }
}

export class AddResponse implements Action {
    readonly type = ResponseActionTypes.AddResponse;

    constructor (public payload: { response: Response }) { }
}

export class UpsertResponse implements Action {
    readonly type = ResponseActionTypes.UpsertResponse;

    constructor (public payload: { response: Response }) { }
}

export class AddResponses implements Action {
    readonly type = ResponseActionTypes.AddResponses;

    constructor (public payload: { responses: Response[] }) { }
}

export class UpsertResponses implements Action {
    readonly type = ResponseActionTypes.UpsertResponses;

    constructor (public payload: { responses: Response[] }) { }
}

export class UpdateResponse implements Action {
    readonly type = ResponseActionTypes.UpdateResponse;

    constructor (public payload: { response: Update<Response> }) { }
}

export class UpdateResponseAsync implements Action {
    readonly type = ResponseActionTypes.UpdateResponseAsync;

    constructor (public payload: { response: Response, loading: LoadingType, activateResponse?: boolean, actions?: AddEditAction[], isCloseAction?: boolean, }) { }
}

export class UpdateResponses implements Action {
    readonly type = ResponseActionTypes.UpdateResponses;

    constructor (public payload: { responses: Update<Response>[] }) { }
}

export class DeleteResponse implements Action {
    readonly type = ResponseActionTypes.DeleteResponse;

    constructor (public payload: { id: string }) { }
}

export class DeleteResponses implements Action {
    readonly type = ResponseActionTypes.DeleteResponses;

    constructor (public payload: { ids: string[] }) { }
}

export class ClearResponses implements Action {
    readonly type = ResponseActionTypes.ClearResponses;
}

export type ResponseActions =
    LoadResponses
    | AddResponse
    | UpsertResponse
    | AddResponses
    | UpsertResponses
    | UpdateResponse
    | UpdateResponses
    | DeleteResponse
    | DeleteResponses
    | ClearResponses
    | PostNewResponse
    | PostNewResponseError
    | SelectActiveResponse
    | SignalRCloseResponse
    | SignalRUpdateResponse
    | SignalRNewResponse
    | ShowSelectingLocation
    | ShowManageResponse
    | UpdateResponseActions
    | UpdateResponseActionsError
    | UpdateResponseActionsSuccess
    | SignalRUpdateResponseAction
    | ActivateResponseActionPlan
    | PostNewResponseSuccess;
