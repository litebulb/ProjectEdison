import { Update } from '@ngrx/entity';
import { Action } from '@ngrx/store';

import { Device } from './device.model';

export enum DeviceActionTypes {
    LoadDevices = '[Device] Load Devices',
    AddDevice = '[Device] Add Device',
    UpsertDevice = '[Device] Upsert Device',
    AddDevices = '[Device] Add Devices',
    UpsertDevices = '[Device] Upsert Devices',
    UpdateDevice = '[Device] Update Device',
    UpdateDevices = '[Device] Update Devices',
    DeleteDevice = '[Device] Delete Device',
    DeleteDevices = '[Device] Delete Devices',
    ClearDevices = '[Device] Clear Devices',
    GetDevices = '[Device] Get Devices',
    GetDevicesError = '[Device] Get Devices Error',
    SignalRUpdateDevice = '[Device] SignalR Update Device',
    SignalRNewDevice = '[Device] SignalR New Device',
    SignalRDeleteDevice = '[Device] SignalR Delete Device',
    TestDevice = '[Device] Test Device',
    TestDeviceSuccess = '[Device] Test Device Success',
    TestDeviceError = '[Device] Test Device Error',
    FocusDevices = '[Device] Focus Devices'
}

export class FocusDevices implements Action {
    readonly type = DeviceActionTypes.FocusDevices;

    constructor (public payload: { devices: Device[] }) { }
}

export class TestDeviceSuccess implements Action {
    readonly type = DeviceActionTypes.TestDeviceSuccess;
}

export class TestDeviceError implements Action {
    readonly type = DeviceActionTypes.TestDeviceError;
}

export class TestDevice implements Action {
    readonly type = DeviceActionTypes.TestDevice;

    constructor (public payload: { deviceId: string }) { }
}

export class SignalRUpdateDevice implements Action {
    readonly type = DeviceActionTypes.SignalRUpdateDevice;

    constructor (public payload: { device: Update<Device> }) { }
}

export class SignalRNewDevice implements Action {
    readonly type = DeviceActionTypes.SignalRNewDevice;

    constructor (public payload: { device: Device }) { }
}

export class SignalRDeleteDevice implements Action {
    readonly type = DeviceActionTypes.SignalRDeleteDevice;

    constructor (public payload: { id: string }) { }
}

export class GetDevicesError implements Action {
    readonly type = DeviceActionTypes.GetDevicesError;
}

export class GetDevices implements Action {
    readonly type = DeviceActionTypes.GetDevices;
}

export class LoadDevices implements Action {
    readonly type = DeviceActionTypes.LoadDevices;

    constructor (public payload: { devices: Device[] }) { }
}

export class AddDevice implements Action {
    readonly type = DeviceActionTypes.AddDevice;

    constructor (public payload: { device: Device }) { }
}

export class UpsertDevice implements Action {
    readonly type = DeviceActionTypes.UpsertDevice;

    constructor (public payload: { device: Device }) { }
}

export class AddDevices implements Action {
    readonly type = DeviceActionTypes.AddDevices;

    constructor (public payload: { devices: Device[] }) { }
}

export class UpsertDevices implements Action {
    readonly type = DeviceActionTypes.UpsertDevices;

    constructor (public payload: { devices: Device[] }) { }
}

export class UpdateDevice implements Action {
    readonly type = DeviceActionTypes.UpdateDevice;

    constructor (public payload: { device: Update<Device> }) { }
}

export class UpdateDevices implements Action {
    readonly type = DeviceActionTypes.UpdateDevices;

    constructor (public payload: { devices: Update<Device>[] }) { }
}

export class DeleteDevice implements Action {
    readonly type = DeviceActionTypes.DeleteDevice;

    constructor (public payload: { id: string }) { }
}

export class DeleteDevices implements Action {
    readonly type = DeviceActionTypes.DeleteDevices;

    constructor (public payload: { ids: string[] }) { }
}

export class ClearDevices implements Action {
    readonly type = DeviceActionTypes.ClearDevices;
}

export type DeviceActions =
    LoadDevices
    | AddDevice
    | UpsertDevice
    | AddDevices
    | UpsertDevices
    | UpdateDevice
    | UpdateDevices
    | DeleteDevice
    | DeleteDevices
    | ClearDevices
    | SignalRNewDevice
    | SignalRUpdateDevice
    | SignalRDeleteDevice;
