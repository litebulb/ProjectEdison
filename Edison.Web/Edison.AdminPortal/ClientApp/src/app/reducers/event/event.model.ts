import { GeoLocation } from '../../core/models/geoLocation';
import { Device } from '../device/device.model';

export interface EventInstance {
    date: number;
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
    startDate: number;
    closureDate: number;
    endDate: number;
    updateDate?: number;
    geolocation?: GeoLocation;
}
