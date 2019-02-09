import { ToastrService } from 'ngx-toastr';
import { Observable, of, Subscriber } from 'rxjs';
import { catchError, map, mergeMap, withLatestFrom } from 'rxjs/operators';
import UUIDv1 from 'uuid/v1';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { Action, Store } from '@ngrx/store';

import { environment } from '../../environments/environment';
import { AppState } from '../reducers';
import { SelectActionPlan } from '../reducers/action-plan/action-plan.actions';
import {
  ActionChangeType,
  ActionPlanAction,
  ActionPlanType,
  ActionStatus,
} from '../reducers/action-plan/action-plan.model';
import { SelectActiveEvent } from '../reducers/event/event.actions';
import {
  ActivateResponseActionPlan,
  AddLocationToActiveResponse,
  AddLocationToActiveResponseError,
  AddLocationToActiveResponseSuccess,
  AddResponse,
  CloseResponse,
  CloseResponseError,
  DontShowSelectingLocation,
  GetResponse,
  GetResponseError,
  GetResponsesError,
  LoadingType,
  LoadResponses,
  PostNewResponse,
  PostNewResponseError,
  PostNewResponseSuccess,
  PutResponse,
  PutResponseError,
  ResponseActionTypes,
  ResponseNonAction,
  RetryResponseActions,
  RetryResponseActionsError,
  RetryResponseActionsSuccess,
  SelectActiveResponse,
  ShowActivateResponse,
  ShowSelectingLocation,
  SignalRUpdateResponseAction,
  UpdateResponse,
  UpdateResponseActions,
  UpdateResponseActionsError,
  UpdateResponseActionsSuccess,
  UpdateResponseAsync,
} from '../reducers/response/response.actions';
import { Response } from '../reducers/response/response.model';
import { selectAll } from '../reducers/response/response.reducer';

const _getSuccessMessage = (actionPlanAction: ActionPlanAction) => {
  switch (actionPlanAction.actionType) {
    case ActionPlanType.LightSensor:
      return `${actionPlanAction.parameters.radius.replace(/^\w/, c =>
        c.toUpperCase()
      )} radius lights activated.`;
    case ActionPlanType.Notification:
      return 'Notification sent successfully.';
    case ActionPlanType.EmergencyCall:
    case ActionPlanType.Twilio:
      return '911 Call initiated successfully.';
    case ActionPlanType.Email:
      return 'Email sent successfully.';
    default:
      return null;
  }
};

const _getFailureMessage = (actionPlanAction: ActionPlanAction) => {
  switch (actionPlanAction.actionType) {
    case ActionPlanType.LightSensor:
      return `${actionPlanAction.parameters.radius.replace(/^\w/, c =>
        c.toUpperCase()
      )} radius lights failed.`;
    case ActionPlanType.Notification:
      return 'Notification failed to send.';
    case ActionPlanType.EmergencyCall:
    case ActionPlanType.Twilio:
      return '911 Call failed.';
    case ActionPlanType.Email:
      return 'Email failed to send.';
    default:
      return null;
  }
};

const _setActionsLoading = (
  actions: ActionPlanAction[],
  locationActionsOnly?: boolean
) => {
  if (actions) {
    return actions.map(action => {
      if (
        action.status !== ActionStatus.Success &&
        (!locationActionsOnly ||
          action.actionType === ActionPlanType.LightSensor)
      ) {
        return { ...action, loading: true, status: null };
      }

      return action;
    });
  }
  return actions;
};

@Injectable()
export class ResponseEffects {
  private _toastAction(foundAction, respToUpdate) {
    if (foundAction) {
      const toastrOptions = {
        progressBar: true,
        closeButton: true,
      };
      switch (foundAction.status) {
        case ActionStatus.Error:
        case ActionStatus.Unknown:
          const failMsg = _getFailureMessage(foundAction);
          if (failMsg) {
            this.toastr.error(
              failMsg,
              respToUpdate.actionPlan.name,
              toastrOptions
            );
          }
          break;
        case ActionStatus.NotStarted:
        case ActionStatus.Skipped:
          break;
        case ActionStatus.Success:
          const successMsg = _getSuccessMessage(foundAction);
          if (successMsg) {
            this.toastr.success(
              successMsg,
              respToUpdate.actionPlan.name,
              toastrOptions
            );
          }
          break;
      }
    }
  }

  @Effect()
  getResponses$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.GetResponses),
    mergeMap(action =>
      environment.mockData
        ? new Observable<Action>((sub: Subscriber<Action>) =>
            sub.next(new LoadResponses({ responses: [] }))
          )
        : this.http
            .get(`${environment.baseUrl}${environment.apiUrl}responses`)
            .pipe(
              map((responses: Response[]) => {
                return responses
                  ? new LoadResponses({ responses })
                  : new GetResponsesError();
              }),
              catchError(() => of(new GetResponsesError()))
            )
    )
  );

  @Effect()
  getResponse$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.GetResponse),
    mergeMap((action: GetResponse) =>
      this.http
        .get(
          `${environment.baseUrl}${environment.apiUrl}responses/${
            action.payload.responseId
          }`
        )
        .pipe(
          map((response: Response) =>
            response
              ? new UpdateResponse({
                  response: { id: response.responseId, changes: response },
                })
              : new GetResponseError()
          ),
          catchError(() => of(new GetResponseError()))
        )
    )
  );

  @Effect()
  postNewResponse$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.PostNewResponse),
    withLatestFrom(this.store$),
    map(([action, { auth: { user } }]) => {
      const {
        payload: { event, actionPlan },
      } = action as PostNewResponse;
      return new Response(
        event,
        actionPlan,
        user,
        environment.mockData ? UUIDv1() : null
      );
    }),
    mergeMap((response: Response) => {
      return environment.mockData
        ? new Observable<Action>((sub: Subscriber<Action>) =>
            sub.next(
              new AddResponse({
                response: { ...response, eventClusterIds: [] },
              })
            )
          )
        : this.http
            .post(
              `${environment.baseUrl}${environment.apiUrl}responses`,
              response
            )
            .pipe(
              map((response: Response) =>
                response
                  ? new PostNewResponseSuccess({ response })
                  : new PostNewResponseError()
              ),
              catchError(() => of(new PostNewResponseError()))
            );
    })
  );

  @Effect()
  showSelectingLocation$: Observable<Action> = this.actions$.pipe(
    ofType(
      ResponseActionTypes.AddResponse,
      ResponseActionTypes.SignalRNewResponse
    ),
    map((action: AddResponse) => {
      if (!action.payload.response.geolocation) {
        return new ShowSelectingLocation({
          showSelectingLocation: action.payload.response.primaryEventClusterId
            ? false
            : true,
          response: action.payload.response,
        });
      }

      return new DontShowSelectingLocation();
    })
  );

  @Effect()
  setActiveResponseOnShow$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.ShowSelectingLocation),
    map(
      ({ payload: { response } }: ShowSelectingLocation) =>
        new SelectActiveResponse({ response })
    )
  );

  @Effect()
  putResponse$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.PutResponse),
    mergeMap(({ payload }: PutResponse) =>
      environment.mockData
        ? new Observable<Action>((sub: Subscriber<Action>) =>
            sub.next(
              new UpdateResponse({
                response: {
                  id: payload.response.responseId,
                  changes: payload.response,
                },
              })
            )
          )
        : this.http
            .put(
              `${environment.baseUrl}${environment.apiUrl}responses`,
              payload.response
            )
            .pipe(
              map((response: Response) =>
                response
                  ? new UpdateResponse({
                      response: {
                        id: payload.response.responseId,
                        changes: response,
                      },
                    })
                  : new PutResponseError()
              ),
              catchError(() => of(new PutResponseError()))
            )
    )
  );

  @Effect()
  updateResponseActions$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.UpdateResponseActions),
    mergeMap(
      ({
        payload: { response, actions, isCloseAction },
      }: UpdateResponseActions) =>
        this.http
          .put(
            `${environment.baseUrl}${environment.apiUrl}responses/changeaction`,
            {
              responseId: response.responseId,
              actions: actions.map(addEditAction => {
                const action = { ...addEditAction.action };
                delete action.loading;
                return { ...addEditAction, isCloseAction, action };
              }),
            }
          )
          .pipe(
            map((response: Response) => {
              return response
                ? new UpdateResponse({
                    response: { id: response.responseId, changes: response },
                  })
                : new UpdateResponseActionsError();
            }),
            catchError(() => of(new UpdateResponseActionsError()))
          )
    )
  );

  @Effect()
  closeResponse$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.CloseResponse),
    mergeMap(({ payload }: CloseResponse) => {
      return environment.mockData
        ? new Observable<Action>((sub: Subscriber<Action>) =>
            sub.next(
              new UpdateResponse({
                response: {
                  id: payload.responseId,
                  changes: {
                    responseState: payload.state,
                  },
                },
              })
            )
          )
        : this.http
            .put(
              `${environment.baseUrl}${environment.apiUrl}responses/close`,
              payload
            )
            .pipe(
              map((response: Response) =>
                response
                  ? new UpdateResponseAsync({
                      response,
                      loading: LoadingType.Closed,
                    })
                  : new CloseResponseError()
              ),
              catchError(() => of(new CloseResponseError()))
            );
    })
  );

  @Effect()
  updateResponseAsync$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.UpdateResponseAsync),
    map((action: UpdateResponseAsync) => {
      const { response, loading, actions, isCloseAction } = action.payload;
      if (response) {
        let openActions = response.actionPlan.openActions;
        let closeActions = response.actionPlan.closeActions;

        if (actions) {
          if (isCloseAction) {
            closeActions = closeActions.map(action => {
              const foundAction = actions.find(
                addEditAction =>
                  addEditAction.action.actionId === action.actionId
              );
              if (foundAction) {
                return { ...foundAction.action, loading: true, status: null };
              }
              return action;
            });

            actions.forEach(addEditAction => {
              const foundAction = closeActions.some(
                ca => ca.actionId === addEditAction.action.actionId
              );
              if (!foundAction) {
                closeActions.push({
                  ...addEditAction.action,
                  loading: true,
                  status: null,
                });
              }
            });
          } else {
            openActions = openActions.map(action => {
              const foundAction = actions.find(
                addEditAction =>
                  addEditAction.action.actionId === action.actionId
              );
              if (foundAction) {
                return { ...foundAction.action, loading: true, status: null };
              }
              return action;
            });

            actions.forEach(addEditAction => {
              const foundAction = openActions.some(
                ca => ca.actionId === addEditAction.action.actionId
              );
              if (!foundAction) {
                openActions.push({
                  ...addEditAction.action,
                  loading: true,
                  status: null,
                });
              }
            });
          }
        } else {
          switch (loading) {
            case LoadingType.All:
              closeActions = _setActionsLoading(closeActions);
              openActions = _setActionsLoading(openActions);
              break;
            case LoadingType.Closed:
              closeActions = _setActionsLoading(closeActions);
              break;
            case LoadingType.Open:
              openActions = _setActionsLoading(openActions);
              break;
          }
        }

        return new UpdateResponse({
          response: {
            id: response.responseId,
            changes: {
              ...response,
              actionPlan: {
                ...response.actionPlan,
                closeActions,
                openActions,
              },
            },
          },
        });
      }

      return new ResponseNonAction();
    })
  );

  @Effect()
  updateActiveResponseLocation$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.AddLocationToActiveResponse),
    mergeMap(
      ({ payload: { location, responseId } }: AddLocationToActiveResponse) => {
        return environment.mockData
          ? new Observable<Action>((sub: Subscriber<Action>) =>
              sub.next(new AddLocationToActiveResponseSuccess())
            )
          : this.http
              .post(
                `${environment.baseUrl}${environment.apiUrl}responses/locate`,
                {
                  responseId: responseId,
                  geolocation: location,
                }
              )
              .pipe(
                map(() => new AddLocationToActiveResponseSuccess()),
                catchError(() => of(new AddLocationToActiveResponseError()))
              );
      }
    )
  );

  @Effect()
  showActivateResponse$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.ShowActivateResponse),
    map(
      ({ payload: { event } }: ShowActivateResponse) =>
        new SelectActiveEvent({ event })
    )
  );

  @Effect()
  updateResponseAction$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.SignalRUpdateResponseAction),
    withLatestFrom(this.store$),
    map(([action, { response }]) => ({
      action,
      responses: selectAll(response),
    })),
    map(
      ({
        action,
        responses,
      }: {
        action: SignalRUpdateResponseAction;
        responses: Response[];
      }) => {
        const { responseId, actionId } = action.payload.message;
        const respToUpdate = responses.find(r => r.responseId === responseId);
        if (respToUpdate && respToUpdate.actionPlan) {
          const { openActions, closeActions } = respToUpdate.actionPlan;
          let foundActionIndex = null;
          if (openActions) {
            foundActionIndex = openActions.findIndex(
              oa => oa.actionId === actionId
            );
            if (foundActionIndex) {
              openActions[foundActionIndex] = {
                ...openActions[foundActionIndex],
                ...action.payload.message,
              };
              this._toastAction(openActions[foundActionIndex], respToUpdate);
            }
          }

          if (closeActions) {
            foundActionIndex = closeActions.findIndex(
              oa => oa.actionId === actionId
            );
            if (foundActionIndex) {
              closeActions[foundActionIndex] = {
                ...closeActions[foundActionIndex],
                ...action.payload.message,
              };
              this._toastAction(closeActions[foundActionIndex], respToUpdate);
            }
          }

          return new ActivateResponseActionPlan({
            response: {
              ...respToUpdate,
              actionPlan: {
                ...respToUpdate.actionPlan,
                openActions,
                closeActions,
              },
            },
          });
        } else {
          return new ActivateResponseActionPlan({
            response: respToUpdate,
          });
        }
      }
    )
  );

  @Effect()
  activateResponseActionPlan$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.ActivateResponseActionPlan),
    map(
      (action: ActivateResponseActionPlan) =>
        new SelectActionPlan({
          actionPlan: action.payload.response.actionPlan,
        })
    )
  );

  @Effect()
  updateResponseActionResponse$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.ActivateResponseActionPlan),
    map(
      (action: ActivateResponseActionPlan) =>
        new UpdateResponse({
          response: {
            id: action.payload.response.responseId,
            changes: action.payload.response,
          },
        })
    )
  );

  @Effect()
  retryResponseActions$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.RetryResponseActions),
    mergeMap((action: RetryResponseActions) =>
      this.http
        .put(
          `${environment.baseUrl}${environment.apiUrl}responses/retryactions`,
          action.payload
        )
        .pipe(
          map(
            (response: Response) =>
              new UpdateResponse({
                response: { id: response.responseId, changes: response },
              })
          ),
          catchError(() => of(new RetryResponseActionsError()))
        )
    )
  );

  @Effect()
  setResponseActionsLoadingOnRetry$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.RetryResponseActions),
    withLatestFrom(this.store$),
    map(([action, { response }]) => ({
      action,
      responses: selectAll(response),
    })),
    map(
      ({
        action,
        responses,
      }: {
        action: RetryResponseActions;
        responses: Response[];
      }) => {
        const response = responses.find(
          resp => resp.responseId === action.payload.responseId
        );
        if (response && response.actionPlan) {
          return new UpdateResponseAsync({
            response,
            loading: LoadingType.All,
          });
        }

        return new ResponseNonAction();
      }
    )
  );

  @Effect()
  setResponseActionsLoadingOnUpdate$: Observable<Action> = this.actions$.pipe(
    ofType(ResponseActionTypes.UpdateResponseActions),
    map(
      ({
        payload: { response, actions, isCloseAction },
      }: UpdateResponseActions) => {
        if (response && response.actionPlan) {
          return new UpdateResponseAsync({
            response,
            loading: LoadingType.All,
            actions,
            isCloseAction,
          });
        }

        return new ResponseNonAction();
      }
    )
  );

  constructor(
    private actions$: Actions,
    private http: HttpClient,
    private store$: Store<AppState>,
    private toastr: ToastrService
  ) {}
}
