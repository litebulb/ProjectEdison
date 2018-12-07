import { Event, EventType } from '../reducers/event/event.model';
import { mockDevices } from './mockDevices';

const oldDate = new Date()
oldDate.setFullYear(2017)

export const mockEvents: Event[] = [
    // {
    //   eventClusterId: 'message1',
    //   eventType: 'message',
    //   device: mockDevices[1],
    //   eventCount: 2,
    //   events: [{ date: new Date().getTime() }, { date: new Date().getTime() }],
    //   startDate: new Date().getTime(),
    //   closureDate: null,
    //   endDate: null,
    // },
    {
        eventClusterId: 'blah',
        eventType: EventType.SoundSensor,
        device: mockDevices[ 0 ],
        eventCount: 1,
        events: [ { date: new Date() } ],
        startDate: new Date(),
        closureDate: oldDate,
        endDate: new Date(),
    },
    {
        eventClusterId: 'blah11',
        eventType: EventType.SoundSensor,
        device: mockDevices[ 0 ],
        eventCount: 1,
        events: [ { date: new Date() } ],
        startDate: new Date(),
        closureDate: oldDate,
        endDate: new Date(),
    },
    {
        eventClusterId: 'blah111',
        eventType: EventType.SoundSensor,
        device: mockDevices[ 0 ],
        eventCount: 1,
        events: [ { date: new Date() } ],
        startDate: new Date(),
        closureDate: new Date(),
        endDate: new Date(),
    },
    {
        eventClusterId: 'blah2',
        eventType: EventType.SoundSensor,
        device: mockDevices[ 0 ],
        eventCount: 1,
        events: [ { date: oldDate } ],
        startDate: oldDate,
        closureDate: new Date(),
        endDate: new Date(),
    },
    {
        eventClusterId: 'blah3',
        eventType: EventType.SoundSensor,
        device: mockDevices[ 1 ],
        eventCount: 2,
        events: [ { date: new Date() }, { date: new Date() } ],
        startDate: new Date(),
        closureDate: new Date(),
        endDate: new Date(),
    },
    {
        eventClusterId: 'blah4',
        eventType: EventType.Message,
        device: {
            deviceId: 'pin4',
            deviceType: 'Mobile',
            online: true,
            name: 'DeVry University',
            location1: '236 W Washington St, Chicago, IL, 60606',
            location2: 'Floor 3',
            location3: 'Room 6',
            geolocation: {
                longitude: -73.975708,
                latitude: 40.76573,
            }
        },
        eventCount: 1,
        events: [ {
            date: new Date(),
            metadata: {
                userId: 'blah',
                username: 'Jarrod Mayer',
                reportType: 'emergency',
                message: 'Oh no something happened!'
            }
        } ],
        startDate: new Date(),
        closureDate: new Date(),
        endDate: new Date(),
    },
    {
        eventClusterId: 'e01965a5-124d-4490-8c44-3be892705124',
        eventType: EventType.Message,
        device: {
            deviceId: '225445d3-25e2-4a2e-8f35-6a810d0cb35e',
            deviceType: 'Mobile',
            name: 'mobile_apns_6349fa30-e34c-44d4-a8ca-c09686877a80',
            location1: null,
            location2: null,
            location3: null,
            geolocation: {
                longitude: -122.406417,
                latitude: 42.785834
            }
        },
        eventCount: 3,
        events: [
            {
                date: new Date(1542292672),
                metadata: {
                    userId: 'dl_alexanderikatz@gmail.com',
                    username: 'Alex Katz',
                    reportType: '5524c361-540f-424f-8a56-23165fb515c6',
                    message: 'Air Quality'
                }
            },
            {
                date: new Date(1542292667),
                metadata: {
                    userId: 'dl_alexanderikatz@gmail.com',
                    username: 'Alex Katz',
                    reportType: '32d4bc56-3c39-4000-9355-c08068a2ba1a',
                    message: 'Active Shooter'
                }
            },
            {
                date: new Date(1542292666),
                metadata: {
                    userId: 'dl_alexanderikatz@gmail.com',
                    username: 'Alex Katz',
                    reportType: 'e768ab86-64d4-42e0-bd8a-9f012f774bbf',
                    message: 'Fire'
                }
            }
        ],
        startDate: new Date(1542292676),
        closureDate: new Date(1542292672),
        endDate: null,
        updateDate: new Date(1542292672),
    },
    {
        eventClusterId: 'e01965a5-124d-4490-8c44-3be892705125',
        eventType: EventType.Message,
        device: {
            deviceId: '225445d3-25e2-4a2e-8f35-6a810d0cb35e',
            deviceType: 'Mobile',
            name: 'mobile_apns_6349fa30-e34c-44d4-a8ca-c09686877a80',
            location1: null,
            location2: null,
            location3: null,
            geolocation: {
                longitude: -122.406417,
                latitude: 42.785834
            }
        },
        eventCount: 3,
        events: [
            {
                date: new Date(1542292672),
                metadata: {
                    userId: 'dl_alexanderikatz@gmail.com',
                    username: 'Alex Katz',
                    reportType: '5524c361-540f-424f-8a56-23165fb515c6',
                    message: 'Air Quality'
                }
            },
            {
                date: new Date(1542292667),
                metadata: {
                    userId: 'dl_alexanderikatz@gmail.com',
                    username: 'Alex Katz',
                    reportType: '32d4bc56-3c39-4000-9355-c08068a2ba1a',
                    message: 'Active Shooter'
                }
            },
            {
                date: new Date(1542292666),
                metadata: {
                    userId: 'dl_alexanderikatz@gmail.com',
                    username: 'Alex Katz',
                    reportType: 'e768ab86-64d4-42e0-bd8a-9f012f774bbf',
                    message: 'Fire'
                }
            }
        ],
        startDate: new Date(1542292686),
        closureDate: new Date(1542292672),
        endDate: null,
        updateDate: new Date(1542292672),
    },
    {
        eventClusterId: 'e01965a5-124d-4490-8c44-3be892705126',
        eventType: EventType.Message,
        device: {
            deviceId: '225445d3-25e2-4a2e-8f35-6a810d0cb35e',
            deviceType: 'Mobile',
            name: 'mobile_apns_6349fa30-e34c-44d4-a8ca-c09686877a80',
            location1: null,
            location2: null,
            location3: null,
            geolocation: {
                longitude: -122.406417,
                latitude: 42.785834
            }
        },
        eventCount: 3,
        events: [
            {
                date: new Date(1542292672),
                metadata: {
                    userId: 'dl_alexanderikatz@gmail.com',
                    username: 'Alex Katz',
                    reportType: '5524c361-540f-424f-8a56-23165fb515c6',
                    message: 'Air Quality'
                }
            },
            {
                date: new Date(1542292667),
                metadata: {
                    userId: 'dl_alexanderikatz@gmail.com',
                    username: 'Alex Katz',
                    reportType: '32d4bc56-3c39-4000-9355-c08068a2ba1a',
                    message: 'Active Shooter'
                }
            },
            {
                date: new Date(1542292666),
                metadata: {
                    userId: 'dl_alexanderikatz@gmail.com',
                    username: 'Alex Katz',
                    reportType: 'e768ab86-64d4-42e0-bd8a-9f012f774bbf',
                    message: 'Fire'
                }
            }
        ],
        startDate: new Date(1542292696),
        closureDate: new Date(),
        endDate: new Date(),
        updateDate: new Date(1542292672),
    }
]
