import { GeoLocation } from '../../core/models/geoLocation';
import { ActionPlan } from '../action-plan/action-plan.model';
import { Event } from '../event/event.model';

export enum ResponseState {
    Inactive,
    Active
}

export class Response {
    responseId: string;
    responderUserId: string;
    responseState: ResponseState;
    startDate: string;
    endDate: string;
    actionPlan: ActionPlan;
    actionPlanId: string;
    primaryEventClusterId?: string;
    geolocation: GeoLocation;
    eventClusterIds: string[];
    event?: Event;
    name: string;
    icon: string;
    color: string;
    delayStart: boolean = false;

    constructor (event: Event, actionPlan: ActionPlan, user, responseId?: string) {
        this.responderUserId = user.profile[ 'oid' ];
        this.actionPlanId = actionPlan.actionPlanId;
        this.responseState = ResponseState.Active;
        this.eventClusterIds = [];
        this.name = actionPlan.name;
        this.icon = actionPlan.icon;
        this.color = actionPlan.color;
        this.actionPlan = actionPlan;

        if (event) {
            this.primaryEventClusterId = event.eventClusterId;
            this.geolocation = event.device.geolocation;
            this.event = event;
        } else {
            this.delayStart = true;
            this.primaryEventClusterId = null;
        }

        if (responseId) {
            this.responseId = responseId;
        }
    }
}
