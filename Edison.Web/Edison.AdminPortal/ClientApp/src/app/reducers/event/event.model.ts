import { GeoLocation } from '../../core/models/geoLocation';
import { Device } from '../device/device.model';

export interface EventInstance {
    date: Date;
    metadata?: any;
}

export enum EventType {
    Message = 'message',
    SoundSensor = 'sound',
    Button = 'button'
}

export interface Event {
    eventClusterId: string;
    eventType: EventType;
    device: Device;
    eventCount: number;
    events: EventInstance[];
    startDate: Date;
    closureDate: Date;
    endDate: Date;
    updateDate?: Date;
    geolocation?: GeoLocation;
}
