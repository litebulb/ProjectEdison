import { Device } from "../../../reducers/device/device.model";
import { Response } from "../../../reducers/response/response.model";
import { Event } from "../../../reducers/event/event.model";

export interface ExpandedDevice extends Device {
    response?: Response;
    event?: Event;
}
