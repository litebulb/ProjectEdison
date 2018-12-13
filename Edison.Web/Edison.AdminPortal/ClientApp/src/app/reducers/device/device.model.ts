import { GeoLocation } from '../../core/models/geoLocation';

export interface Device {
    deviceId: string;
    deviceType: string;
    online?: boolean;
    name: string;
    location1: string;
    location2: string;
    location3: string;
    geolocation: GeoLocation;
    lastAccessTime?: string;
}
