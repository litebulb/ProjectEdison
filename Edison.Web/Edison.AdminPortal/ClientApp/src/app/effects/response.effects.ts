import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { Observable, of, Subscriber } from 'rxjs';
import { Action, Store } from '@ngrx/store';
import { mergeMap, catchError, map, withLatestFrom } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import UUIDv1 from 'uuid/v1';
import {
  ResponseActionTypes,
  PostNewResponse,
  AddResponse,
  PostNewResponseError,
  UpdateResponse,
  PutResponse,
  PutResponseError,
  LoadResponses,
  GetResponsesError,
  GetResponse,
  GetResponseError,
  CloseResponse,
  CloseResponseError,
} from '../reducers/response/response.actions';
import { AppState } from '../reducers';
import { Response } from '../reducers/response/response.model';

@Injectable()
export class ResponseEffects {
  @Effect()
  getResponses$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.GetResponses),
    mergeMap(
      action =>
        environment.mockData
          ? new Observable<Action>((sub: Subscriber<Action>) =>
              sub.next(new LoadResponses({ responses: [] }))
            )
          : this.http.get(`${environment.apiUrl}responses`).pipe(
              map(
                (responses: Response[]) => {
                    return responses ? new LoadResponses({ responses }) : new GetResponsesError();
                  }
              ),
              catchError(() => of(new GetResponsesError()))
            )
    )
  );

  @Effect()
  getResponse$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.GetResponse),
    mergeMap((action: GetResponse) => this.http.get(`${environment.apiUrl}responses/${action.payload.responseId}`).pipe(
        map(
          (response: Response) =>
          response ? new UpdateResponse({ response: { id: response.responseId, changes: response } }) : new GetResponseError()
        ),
        catchError(() => of(new GetResponseError()))
      ))
  );

  @Effect()
  postNewResponse$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.PostNewResponse),
    withLatestFrom(this.store$),
    map(([action, { auth: { user } }]) => {
      const {
        payload: { event, actionPlan },
      } = action as PostNewResponse;
      return new Response(event, actionPlan, user, UUIDv1());
    }),
    mergeMap(
      (newResponse: Response) =>
        environment.mockData
          ? new Observable<Action>((sub: Subscriber<Action>) =>
              sub.next(new AddResponse({ response: {
                ...newResponse,
                name: newResponse.actionPlan.name,
                icon: newResponse.actionPlan.icon,
                color: newResponse.actionPlan.color,
                eventClusterIds: ['blah3']
              } }))
            )
          : this.http.post(`${environment.apiUrl}responses`, {
            responderUserId: newResponse.responderUserId,
            actionPlan: newResponse.actionPlan,
            primaryEventClusterId: newResponse.primaryEventClusterId,
            geolocation: newResponse.geolocation,
            name: newResponse.actionPlan.name,
            icon: newResponse.actionPlan.icon,
            color: newResponse.actionPlan.color,
          }).pipe(
              map(
                (response: Response) =>
                  response
                    ? new AddResponse({ response: {
                      ...response,
                      actionPlan: newResponse.actionPlan,
                      name: newResponse.actionPlan.name,
                      icon: newResponse.actionPlan.icon,
                      color: newResponse.actionPlan.color,
                    } })
                    : new PostNewResponseError()
              ),
              catchError(() => of(new PostNewResponseError()))
            )
    )
  );

  @Effect()
  putResponse$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.PutResponse),
    mergeMap(
      ({ payload }: PutResponse) =>
        environment.mockData
          ? new Observable<Action>((sub: Subscriber<Action>) =>
              sub.next(new UpdateResponse({ response: { id: payload.response.responseId, changes: payload.response } }))
            )
          : this.http.put(`${environment.apiUrl}responses`, payload.response).pipe(
              map(
                (response: Response) =>
                  response
                    ? new UpdateResponse({ response: { id: payload.response.responseId, changes: response } })
                    : new PutResponseError()
              ),
              catchError(() => of(new PutResponseError()))
            )
    )
  );

  @Effect()
  closeResponse$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.CloseResponse),
    mergeMap(
      ({ payload }: CloseResponse) =>
          this.http.put(`${environment.apiUrl}responses/close`, payload).pipe(
              map(
                (response: Response) =>
                  response
                    ? new UpdateResponse({ response: { id: payload.responseId, changes: response } })
                    : new CloseResponseError()
              ),
              catchError(() => of(new CloseResponseError()))
            )
    )
  );

  constructor(
    private actions$: Actions,
    private http: HttpClient,
    private store$: Store<AppState>
  ) {}
}
