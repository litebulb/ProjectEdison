import { GeoLocation } from '../../shared/models/geoLocation';

export interface Device {
  deviceId: string;
  deviceType: string;
  online: boolean;
  locationName: string;
  locationLevel1: string;
  locationLevel2: string;
  locationLevel3: string;
  geolocation: GeoLocation;
}
