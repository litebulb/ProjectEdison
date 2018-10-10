import { Action } from '@ngrx/store';
import { Update } from '@ngrx/entity';
import { Response, ResponseState } from './response.model';
import { Event } from '../event/event.model';
import { ActionPlan } from '../action-plan/action-plan.model';

export enum ResponseActionTypes {
  LoadResponses = '[Response] Load Responses',
  AddResponse = '[Response] Add Response',
  UpsertResponse = '[Response] Upsert Response',
  AddResponses = '[Response] Add Responses',
  UpsertResponses = '[Response] Upsert Responses',
  UpdateResponse = '[Response] Update Response',
  UpdateResponses = '[Response] Update Responses',
  DeleteResponse = '[Response] Delete Response',
  DeleteResponses = '[Response] Delete Responses',
  ClearResponses = '[Response] Clear Responses',
  PostNewResponse = '[Response] Post New Response',
  PostNewResponseError = '[Response] Post New Response Error',
  PutResponse = '[Response] Put Response',
  PutResponseError = '[Response] Put Response Error',
  GetResponses = '[Response] Get Responses',
  GetResponsesError = '[Response] Get Responses Error',
  SelectActiveResponse = '[Response] Select Active Response',
  SignalRNewResponse = '[Event] SignalR New Response',
  SignalRUpdateResponse = '[Event] SignalR Update Response',
  SignalRCloseResponse = '[Event] SignalR Close Response',
  GetResponse = '[Response] Get Response',
  GetResponseError = '[Response] Get Response Error',
  CloseResponse = '[Response] Close Response',
  CloseResponseError = '[Response] Close Response Error',
}

export class CloseResponse implements Action {
  readonly type = ResponseActionTypes.CloseResponse;

  constructor(public payload: { responseId: string, state: ResponseState }) {}
}

export class CloseResponseError implements Action {
  readonly type = ResponseActionTypes.CloseResponseError;
}

export class GetResponse implements Action {
  readonly type = ResponseActionTypes.GetResponse;

  constructor(public payload: { responseId: string }) {}
}

export class GetResponseError implements Action {
  readonly type = ResponseActionTypes.GetResponseError;
}

export class SignalRNewResponse implements Action {
  readonly type = ResponseActionTypes.SignalRNewResponse;

  constructor(public payload: { response: Response }) {}
}

export class SignalRUpdateResponse implements Action {
  readonly type = ResponseActionTypes.SignalRUpdateResponse;

  constructor(public payload: { response: Update<Response> }) {}
}

export class SignalRCloseResponse implements Action {
  readonly type = ResponseActionTypes.SignalRCloseResponse;

  constructor(public payload: { response: Update<Response> }) {}
}

export class SelectActiveResponse implements Action {
  readonly type = ResponseActionTypes.SelectActiveResponse;

  constructor(public payload: { response: Response }) {}
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

  constructor(public payload: { response: Response }) {}
}

export class PostNewResponseError implements Action {
  readonly type = ResponseActionTypes.PostNewResponseError;
}

export class PostNewResponse implements Action {
  readonly type = ResponseActionTypes.PostNewResponse;

  constructor(public payload: { event: Event, actionPlan: ActionPlan }) {}
}

export class LoadResponses implements Action {
  readonly type = ResponseActionTypes.LoadResponses;

  constructor(public payload: { responses: Response[] }) {}
}

export class AddResponse implements Action {
  readonly type = ResponseActionTypes.AddResponse;

  constructor(public payload: { response: Response }) {}
}

export class UpsertResponse implements Action {
  readonly type = ResponseActionTypes.UpsertResponse;

  constructor(public payload: { response: Response }) {}
}

export class AddResponses implements Action {
  readonly type = ResponseActionTypes.AddResponses;

  constructor(public payload: { responses: Response[] }) {}
}

export class UpsertResponses implements Action {
  readonly type = ResponseActionTypes.UpsertResponses;

  constructor(public payload: { responses: Response[] }) {}
}

export class UpdateResponse implements Action {
  readonly type = ResponseActionTypes.UpdateResponse;

  constructor(public payload: { response: Update<Response> }) {}
}

export class UpdateResponses implements Action {
  readonly type = ResponseActionTypes.UpdateResponses;

  constructor(public payload: { responses: Update<Response>[] }) {}
}

export class DeleteResponse implements Action {
  readonly type = ResponseActionTypes.DeleteResponse;

  constructor(public payload: { id: string }) {}
}

export class DeleteResponses implements Action {
  readonly type = ResponseActionTypes.DeleteResponses;

  constructor(public payload: { ids: string[] }) {}
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
 | SignalRNewResponse;
