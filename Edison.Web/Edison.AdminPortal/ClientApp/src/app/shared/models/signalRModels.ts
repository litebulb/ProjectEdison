import { Event } from '../../reducers/event/event.model';
import { SignalRTypes } from './signalRTypes';
import { Device } from '../../reducers/device/device.model';

export namespace SignalRModels {

    export interface EventMessage {
        eventCluster: Event;
        updateType: SignalRTypes.Event;
    }

    export interface DeviceMessage {
        device: Device;
        deviceId: string;
        updateType: SignalRTypes.Device;
    }

    export interface ResponseMessage {
        response: any;
        responseId: string;
        updateType: SignalRTypes.Response;
    }

}
