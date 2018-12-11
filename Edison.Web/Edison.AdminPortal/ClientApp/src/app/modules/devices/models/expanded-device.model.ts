import { Device } from '../../../reducers/device/device.model';
import { Event } from '../../../reducers/event/event.model';
import { Response } from '../../../reducers/response/response.model';

export interface ExpandedDevice extends Device {
    response?: Response;
    event?: Event;
    fullLocationName?: string;
}
