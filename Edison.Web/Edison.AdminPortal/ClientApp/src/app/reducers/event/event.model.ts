import { Device } from '../device/device.model';
import { GeoLocation } from '../../shared/models/geoLocation';

export interface EventInstance {
  date: number;
  metadata?: any;
}

export interface Event {
  eventClusterId: string;
  eventType: string;
  device: Device;
  eventCount: number;
  events: EventInstance[];
  startDate: number;
  closureDate: number;
  endDate: number;
  geolocation?: GeoLocation;
}
