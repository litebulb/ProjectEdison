import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { Observable, of, Subscriber } from 'rxjs';
import { Action } from '@ngrx/store';
import { mergeMap, catchError, map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import {
    LoadDevices,
    GetDevicesError,
    DeviceActionTypes,
    TestDeviceSuccess,
    TestDeviceError,
    TestDevice,
} from '../reducers/device/device.actions';
import { Device } from '../reducers/device/device.model';
import { environment } from '../../environments/environment';
import { mockDevices } from '../mockData/mockDevices';

@Injectable()
export class DeviceEffects {
    @Effect()
    getDevices$: Observable<Action> = this.actions$.pipe(
        ofType(DeviceActionTypes.GetDevices),
        mergeMap(
            action =>
                environment.mockData
                    ? new Observable<Action>((sub: Subscriber<Action>) =>
                        sub.next(new LoadDevices({ devices: mockDevices }))
                    )
                    : this.http.get(`${environment.baseUrl}${environment.apiUrl}devices`).pipe(
                        map(
                            (devices: Device[]) =>
                                devices ? new LoadDevices({ devices }) : new GetDevicesError()
                        ),
                        catchError(() => of(new GetDevicesError()))
                    )
        )
    );

    @Effect()
    testDevice$: Observable<Action> = this.actions$.pipe(
        ofType(DeviceActionTypes.TestDevice),
        mergeMap((action: TestDevice) => this.http.put(`${environment.baseUrl}${environment.apiUrl}IoTHub/DirectMethods`, {
            deviceIds: [ action.payload.deviceId ],
            methodName: 'test',
            payload: null,
        }).pipe(map(() => new TestDeviceSuccess()), catchError(() => of(new TestDeviceError()))))
    )

    constructor (private actions$: Actions, private http: HttpClient) { }
}
