import { mockDevices } from './mockDevices'
import { Event, EventType } from '../reducers/event/event.model'

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
        events: [ { date: new Date().getTime() } ],
        startDate: new Date().getTime(),
        closureDate: null,
        endDate: null,
    },
    {
        eventClusterId: 'blah11',
        eventType: EventType.SoundSensor,
        device: mockDevices[ 0 ],
        eventCount: 1,
        events: [ { date: new Date().getTime() } ],
        startDate: new Date().getTime(),
        closureDate: null,
        endDate: null,
    },
    {
        eventClusterId: 'blah111',
        eventType: EventType.SoundSensor,
        device: mockDevices[ 0 ],
        eventCount: 1,
        events: [ { date: new Date().getTime() } ],
        startDate: new Date().getTime(),
        closureDate: null,
        endDate: null,
    },
    {
        eventClusterId: 'blah2',
        eventType: EventType.SoundSensor,
        device: mockDevices[ 0 ],
        eventCount: 1,
        events: [ { date: oldDate.getTime() } ],
        startDate: oldDate.getTime(),
        closureDate: new Date().getTime(),
        endDate: null,
    },
    {
        eventClusterId: 'blah3',
        eventType: EventType.SoundSensor,
        device: mockDevices[ 1 ],
        eventCount: 2,
        events: [ { date: new Date().getTime() }, { date: new Date().getTime() } ],
        startDate: new Date().getTime(),
        closureDate: null,
        endDate: null,
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
            date: new Date().getTime(),
            metadata: {
                userId: 'blah',
                username: 'Jarrod Mayer',
                reportType: 'emergency',
                message: 'Oh no something happened!'
            }
        } ],
        startDate: new Date().getTime(),
        closureDate: null,
        endDate: null,
    },
]
