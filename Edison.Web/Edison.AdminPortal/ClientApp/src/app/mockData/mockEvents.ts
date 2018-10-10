import { mockDevices } from './mockDevices';
import { Event } from '../reducers/event/event.model';

const oldDate = new Date();
oldDate.setFullYear(2017);

export const mockEvents: Event[] = [
  {
    eventClusterId: 'blah',
    eventType: 'Sound Sensor',
    device: mockDevices[0],
    eventCount: 1,
    events: [{ date: new Date().getTime() }],
    startDate: new Date().getTime(),
    closureDate: null,
    endDate: null,
  },
  {
    eventClusterId: 'blah2',
    eventType: 'Sound Sensor',
    device: mockDevices[0],
    eventCount: 1,
    events: [{ date: oldDate.getTime() }],
    startDate: oldDate.getTime(),
    closureDate: new Date().getTime(),
    endDate: null,
  },
  {
    eventClusterId: 'blah3',
    eventType: 'Button',
    device: mockDevices[1],
    eventCount: 2,
    events: [{ date: new Date().getTime() }, { date: new Date().getTime() }],
    startDate: new Date().getTime(),
    closureDate: null,
    endDate: null,
  },
];
