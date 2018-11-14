import { Device } from '../reducers/device/device.model';

export const mockDevices: Device[] = [
    {
        deviceId: 'pin1',
        deviceType: 'Button',
        online: true,
        name: 'DeVry University',
        location1: '236 W Washington St, Chicago, IL, 60606',
        location2: 'Floor 3',
        location3: 'Room 6',
        geolocation: {
            longitude: -73.985708,
            latitude: 40.75773,
        }
    },
    {
        deviceId: 'pin2',
        deviceType: 'Sound',
        online: true,
        name: 'DeVry University',
        location1: '236 W Washington St, Chicago, IL, 60606',
        location2: 'Floor 3',
        location3: 'Room 6',
        geolocation: {
            longitude: -73.995708,
            latitude: 40.76773,
        }
    },
    {
        deviceId: 'pin3',
        deviceType: 'Button',
        online: true,
        name: 'DeVry University',
        location1: '236 W Washington St, Chicago, IL, 60606',
        location2: 'Floor 3',
        location3: 'Room 6',
        geolocation: {
            longitude: -73.995708,
            latitude: 40.76573,
        }
    }
];
