export enum ActionPlanColor {
  Red = 'Red',
  Green = 'Green',
  Yellow = 'Yellow'
}

export enum ActionPlanIcon {
  Fire = 'Fire',
  Gun = 'Gun',
  Tornado = 'Tornado',
  Package = 'Package',
  Health = 'Health',
  Protest = 'Protest',
  VIP = 'VIP',
  Pollution = 'Pollution'
}

export enum ActionPlanType {
  None,
  Notification = 'notification',
  Email = 'email',
  RapidSOS = 'rapidsos',
  LightSensor = 'lightsensor'
}

export interface ActionPlanAction {
  actionId: string;
  actionType: ActionPlanType;
  isActive: true;
  description: string;
  parameters?: any;
}

export interface ActionPlanTextAction extends ActionPlanAction {
  parameters: {
    content: string;
  };
}

export interface ActionPlanNotificationAction extends ActionPlanAction {
  parameters: {
    content: string;
  };
}

export interface ActionPlanRadiusAction extends ActionPlanAction {
  parameters: {
    radius: string;
    color: string;
  };
}

export type ActionPlanActionTypes =
  ActionPlanNotificationAction
  | ActionPlanRadiusAction
  | ActionPlanTextAction;

export interface ActionPlan {
  actionPlanId: string;
  name: string;
  description: string;
  isActive: true;
  color: ActionPlanColor;
  icon: ActionPlanIcon;
  openActions?: ActionPlanAction[];
  closeActions?: ActionPlanAction[];
}
