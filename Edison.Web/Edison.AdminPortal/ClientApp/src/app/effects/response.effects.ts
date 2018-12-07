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
    ActionPlanAction, ActionPlanType, ActionStatus
} from '../reducers/action-plan/action-plan.model';
import { SelectActiveEvent } from '../reducers/event/event.actions';
import {
    ActivateResponseActionPlan, AddLocationToActiveResponse, AddLocationToActiveResponseError,
    AddLocationToActiveResponseSuccess, AddResponse, CloseResponse, CloseResponseError, GetResponse,
    GetResponseError, GetResponsesError, LoadResponses, PostNewResponse, PostNewResponseError,
    PostNewResponseSuccess, PutResponse, PutResponseError, ResponseActionTypes,
    RetryResponseActions, RetryResponseActionsError, RetryResponseActionsSuccess,
    SelectActiveResponse, ShowActivateResponse, ShowSelectingLocation, SignalRUpdateResponseAction,
    UpdateResponse, UpdateResponseActions, UpdateResponseActionsError, UpdateResponseActionsSuccess
} from '../reducers/response/response.actions';
import { Response } from '../reducers/response/response.model';
import { selectAll } from '../reducers/response/response.reducer';

const getSuccessMessage = (actionPlanAction: ActionPlanAction) => {
    switch (actionPlanAction.actionType) {
        case ActionPlanType.LightSensor:
            return `${actionPlanAction.parameters.radius.replace(/^\w/, c => c.toUpperCase())} radius lights activated.`;
        case ActionPlanType.Notification:
            return 'Notification sent successfully.';
        case ActionPlanType.EmergencyCall:
            return '911 Call initiated successfully.';
        case ActionPlanType.Email:
            return 'Email sent successfully.';
    }
}

const getFailureMessage = (actionPlanAction: ActionPlanAction) => {
    switch (actionPlanAction.actionType) {
        case ActionPlanType.LightSensor:
            return `${actionPlanAction.parameters.radius.replace(/^\w/, c => c.toUpperCase())} radius lights failed.`;
        case ActionPlanType.Notification:
            return 'Notification failed to send.';
        case ActionPlanType.EmergencyCall:
            return '911 Call failed.';
        case ActionPlanType.Email:
            return 'Email failed to send.';
    }
}

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
                    : this.http.get(`${environment.baseUrl}${environment.apiUrl}responses`).pipe(
                        map((responses: Response[]) => {
                            return responses
                                ? new LoadResponses({ responses })
                                : new GetResponsesError()
                        }),
                        catchError(() => of(new GetResponsesError()))
                    )
        )
    )

    @Effect()
    getResponse$: Observable<Action> = this.actions$.pipe(
        ofType(ResponseActionTypes.GetResponse),
        mergeMap((action: GetResponse) =>
            this.http
                .get(`${environment.baseUrl}${environment.apiUrl}responses/${action.payload.responseId}`)
                .pipe(
                    map(
                        (response: Response) =>
                            response
                                ? new UpdateResponse({
                                    response: { id: response.responseId, changes: response },
                                })
                                : new GetResponseError()
                    ),
                    catchError(() => of(new GetResponseError()))
                )
        )
    )

    @Effect()
    postNewResponse$: Observable<Action> = this.actions$.pipe(
        ofType(ResponseActionTypes.PostNewResponse),
        withLatestFrom(this.store$),
        map(([ action, { auth: { user } } ]) => {
            const {
                payload: { event, actionPlan },
            } = action as PostNewResponse
            return new Response(event, actionPlan, user, environment.mockData ? UUIDv1() : null)
        }),
        mergeMap((response: Response) => {

            return environment.mockData
                ? new Observable<Action>((sub: Subscriber<Action>) =>
                    sub.next(
                        new AddResponse({ response: { ...response, eventClusterIds: [] } })
                    )
                )
                : this.http.post(`${environment.baseUrl}${environment.apiUrl}responses`, response).pipe(
                    map(
                        (response: Response) =>
                            response
                                ? new PostNewResponseSuccess({ response })
                                : new PostNewResponseError()
                    ),
                    catchError(() => of(new PostNewResponseError()))
                )
        })
    )

    @Effect()
    showSelectingLocation$: Observable<Action> = this.actions$.pipe(
        ofType(ResponseActionTypes.AddResponse, ResponseActionTypes.SignalRNewResponse),
        map((action: AddResponse) =>
            new ShowSelectingLocation({
                showSelectingLocation: action.payload.response.primaryEventClusterId ? false : true,
                response: action.payload.response
            }))
    )

    @Effect()
    setActiveResponseOnShow$: Observable<Action> = this.actions$.pipe(
        ofType(ResponseActionTypes.ShowSelectingLocation),
        map(({ payload: { response } }: ShowSelectingLocation) =>
            new SelectActiveResponse({ response }))
    )

    @Effect()
    putResponse$: Observable<Action> = this.actions$.pipe(
        ofType(ResponseActionTypes.PutResponse),
        mergeMap(
            ({ payload }: PutResponse) =>
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
                        .put(`${environment.baseUrl}${environment.apiUrl}responses`, payload.response)
                        .pipe(
                            map(
                                (response: Response) =>
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
    )

    @Effect()
    updateResponseActions$: Observable<Action> = this.actions$.pipe(
        ofType(ResponseActionTypes.UpdateResponseActions),
        mergeMap(({ payload: { response, actions } }: UpdateResponseActions) => this.http
            .put(`${environment.baseUrl}${environment.apiUrl}responses/changeaction`, {
                ...response,
                actions
            })
            .pipe(
                map(
                    (response: Response) =>
                        response
                            ? new UpdateResponseActionsSuccess({ response })
                            : new UpdateResponseActionsError()
                ),
                catchError(() => of(new UpdateResponseActionsError()))
            ))
    )

    @Effect()
    responseActionsUpdated$: Observable<Action> = this.actions$.pipe(
        ofType(ResponseActionTypes.UpdateResponseActionsSuccess),
        map(({ payload: { response } }: UpdateResponseActionsSuccess) => new UpdateResponse({
            response: {
                id: response.responseId,
                changes: response,
            }
        }))
    )

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
                : this.http.put(`${environment.baseUrl}${environment.apiUrl}responses/close`, payload).pipe(
                    map(
                        (response: Response) =>
                            response
                                ? new UpdateResponse({
                                    response: { id: payload.responseId, changes: response },
                                })
                                : new CloseResponseError()
                    ),
                    catchError(() => of(new CloseResponseError()))
                )
        })
    )

    @Effect()
    updateActiveResponseLocation$: Observable<Action> = this.actions$.pipe(
        ofType(ResponseActionTypes.AddLocationToActiveResponse),
        mergeMap(({ payload: { location, responseId } }: AddLocationToActiveResponse) => {
            return environment.mockData
                ? new Observable<Action>((sub: Subscriber<Action>) =>
                    sub.next(new AddLocationToActiveResponseSuccess())
                )
                : this.http
                    .post(`${environment.baseUrl}${environment.apiUrl}responses/locate`, {
                        responseId: responseId,
                        geolocation: location
                    })
                    .pipe(
                        map(() => new AddLocationToActiveResponseSuccess()),
                        catchError(() => of(new AddLocationToActiveResponseError()))
                    )
        })
    )

    @Effect()
    showActivateResponse$: Observable<Action> = this.actions$.pipe(
        ofType(ResponseActionTypes.ShowActivateResponse),
        map(({ payload: { event } }: ShowActivateResponse) => new SelectActiveEvent({ event }))
    )

    @Effect()
    updateResponseAction$: Observable<Action> = this.actions$.pipe(
        ofType(ResponseActionTypes.SignalRUpdateResponseAction),
        withLatestFrom(this.store$),
        map(([ action, { response } ]) => ({
            action,
            responses: selectAll(response)
        })),
        map(({ action, responses }: { action: SignalRUpdateResponseAction, responses: Response[] }) => {
            const { responseId, actionId } = action.payload.message;
            const respToUpdate = responses.find(r => r.responseId === responseId);
            if (respToUpdate) {
                const { openActions, closeActions } = respToUpdate.actionPlan;
                let foundAction: ActionPlanAction = null;
                if (openActions) {
                    foundAction = openActions.find(oa => oa.actionId === actionId);
                    if (foundAction) {
                        foundAction = {
                            ...foundAction,
                            ...action.payload.message,
                        }
                    }
                }

                if (closeActions) {
                    foundAction = openActions.find(oa => oa.actionId === actionId);
                    if (foundAction) {
                        foundAction = {
                            ...foundAction,
                            ...action.payload.message,
                        }
                    }
                }

                if (foundAction) {
                    const toastrOptions = {
                        progressBar: true,
                        closeButton: true,
                    }
                    switch (foundAction.status) {
                        case ActionStatus.Error:
                        case ActionStatus.Unknown:
                            this.toastr.error(getFailureMessage(foundAction), respToUpdate.actionPlan.name, toastrOptions);
                            break;
                        case ActionStatus.NotStarted:
                        case ActionStatus.Skipped:
                            break;
                        case ActionStatus.Success:
                            this.toastr.success(getSuccessMessage(foundAction), respToUpdate.actionPlan.name, toastrOptions);
                            break;
                    }
                }

                return new ActivateResponseActionPlan({ response: respToUpdate });
            }
        })
    )

    @Effect()
    activateResponseActionPlan$: Observable<Action> = this.actions$.pipe(
        ofType(ResponseActionTypes.ActivateResponseActionPlan),
        map((action: ActivateResponseActionPlan) => new SelectActionPlan({
            actionPlan: action.payload.response.actionPlan,
        }))
    )

    @Effect()
    updateResponseActionResponse$: Observable<Action> = this.actions$.pipe(
        ofType(ResponseActionTypes.ActivateResponseActionPlan),
        map((action: ActivateResponseActionPlan) => new UpdateResponse({
            response: {
                id: action.payload.response.responseId,
                changes: action.payload.response,
            }
        }))
    )

    @Effect()
    retryResponseActions$: Observable<Action> = this.actions$.pipe(
        ofType(ResponseActionTypes.RetryResponseActions),
        mergeMap((action: RetryResponseActions) =>
            this.http.put(`${environment.baseUrl}${environment.apiUrl}responses/retryactions`, action.payload)
                .pipe(map(() => new RetryResponseActionsSuccess()),
                    catchError(() => of(new RetryResponseActionsError()))
                ))
    )

    constructor (
        private actions$: Actions,
        private http: HttpClient,
        private store$: Store<AppState>,
        private toastr: ToastrService
    ) { }
}
