import { GeoLocation } from '../../core/models/geoLocation';
import { Device } from '../device/device.model';

export interface EventInstance {
    date: string;
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
    startDate: string;
    closureDate: string;
    endDate: string;
    updateDate?: string;
    geolocation?: GeoLocation;
}
