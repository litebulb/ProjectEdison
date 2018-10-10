import { ActionPlan } from '../action-plan/action-plan.model';
import { GeoLocation } from '../../shared/models/geoLocation';
import { Event } from '../event/event.model';
import { User } from 'msal';

export enum ResponseState {
  Inactive,
  Active
}

export class Response {
  responseId: string;
  responderUserId: string;
  responseState: ResponseState;
  startDate: number;
  endDate: number;
  actionPlan: ActionPlan;
  actionPlanId: string;
  primaryEventClusterId: string;
  geolocation: GeoLocation;
  eventClusterIds: string[];
  event?: Event;
  name: string;
  icon: string;
  color: string;

  constructor(event: Event, actionPlan: ActionPlan, user: User, responseId?: string) {
    this.responderUserId = user.idToken['oid'];
    this.actionPlanId = actionPlan.actionPlanId;
    this.primaryEventClusterId = event.eventClusterId;
    this.geolocation = event.device.geolocation;
    this.responseState = ResponseState.Active;
    this.event = event;

    if (responseId) {
      this.responseId = responseId;
      this.actionPlan = actionPlan;
    }
  }
}
