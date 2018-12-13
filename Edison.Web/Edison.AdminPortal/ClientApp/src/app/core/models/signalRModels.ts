import { ActionStatus } from '../../reducers/action-plan/action-plan.model';
import { Device } from '../../reducers/device/device.model';
import { Event } from '../../reducers/event/event.model';
import { Response } from '../../reducers/response/response.model';
import { SignalRTypes } from './signalRTypes';

export namespace SignalRModels {

    export interface ActionMessage {
        actionId: string;
        endDate: string;
        errorMessage: string;
        responseId: string;
        startDate: string;
        status: ActionStatus;
        updateType: SignalRTypes.Action
    }

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
        response: Response;
        responseId: string;
        updateType: SignalRTypes.Response;
    }
}
