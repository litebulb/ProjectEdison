import { Observable, of, Subscriber } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { Action } from '@ngrx/store';

import { environment } from '../../environments/environment';
import { mockEvents } from '../mockData/mockEvents';
import {
    EventActionTypes, GetEventsError, LoadEvents, SelectActiveEvent, ShowEventInEventBar
} from '../reducers/event/event.actions';
import { Event } from '../reducers/event/event.model';

@Injectable()
export class EventEffects {
    @Effect()
    getEvents$: Observable<Action> = this.actions$.pipe(
        ofType(EventActionTypes.GetEvents),
        mergeMap(
            action =>
                environment.mockData
                    ? new Observable<Action>((sub: Subscriber<Action>) =>
                        sub.next(new LoadEvents({ events: mockEvents }))
                    )
                    : this.http.get(`${environment.baseUrl}${environment.apiUrl}eventclusters`).pipe(
                        map(
                            (events: Event[]) =>
                                events ? new LoadEvents({ events }) : new GetEventsError()
                        ),
                        catchError(() => of(new GetEventsError()))
                    )
        )
    );

    @Effect()
    showEventInEventBar$: Observable<Action> = this.actions$.pipe(
        ofType(EventActionTypes.ShowEventInEventBar),
        map(({ payload: { event } }: ShowEventInEventBar) => new SelectActiveEvent({ event }))
    )

    constructor (private actions$: Actions, private http: HttpClient) { }
}
