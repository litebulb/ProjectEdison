import { Device } from '../../../reducers/device/device.model';
import { Event } from '../../../reducers/event/event.model';

export interface MapPin extends Device {
    event?: Event;
    color?: string;
    icon?: string;
}
