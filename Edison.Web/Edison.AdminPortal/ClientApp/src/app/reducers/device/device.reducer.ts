import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Device } from './device.model';
import { DeviceActions, DeviceActionTypes } from './device.actions';

export interface State extends EntityState<Device> {
  // additional entities state properties
}

export const adapter: EntityAdapter<Device> = createEntityAdapter<Device>({
  selectId: (device: Device) => device.deviceId,
});

export const initialState: State = adapter.getInitialState({
  // additional entity state properties
});

export function reducer(
  state = initialState,
  action: DeviceActions
): State {
  switch (action.type) {
    case DeviceActionTypes.SignalRNewDevice:
    case DeviceActionTypes.AddDevice: {
      return adapter.addOne(action.payload.device, state);
    }

    case DeviceActionTypes.UpsertDevice: {
      return adapter.upsertOne(action.payload.device, state);
    }

    case DeviceActionTypes.AddDevices: {
      return adapter.addMany(action.payload.devices, state);
    }

    case DeviceActionTypes.UpsertDevices: {
      return adapter.upsertMany(action.payload.devices, state);
    }

    case DeviceActionTypes.SignalRUpdateDevice:
    case DeviceActionTypes.UpdateDevice: {
      return adapter.updateOne(action.payload.device, state);
    }

    case DeviceActionTypes.UpdateDevices: {
      return adapter.updateMany(action.payload.devices, state);
    }

    case DeviceActionTypes.SignalRDeleteDevice:
    case DeviceActionTypes.DeleteDevice: {
      return adapter.removeOne(action.payload.id, state);
    }

    case DeviceActionTypes.DeleteDevices: {
      return adapter.removeMany(action.payload.ids, state);
    }

    case DeviceActionTypes.LoadDevices: {
      return adapter.addAll(action.payload.devices, state);
    }

    case DeviceActionTypes.ClearDevices: {
      return adapter.removeAll(state);
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
