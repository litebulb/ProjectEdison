export enum ActionPlanColor {
    Red = 'Red',
    Green = 'Green',
    Yellow = 'Yellow',
    Off = 'Off',
}

export enum ActionPlanIcon {
    Fire = 'Fire',
    Gun = 'Gun',
    Tornado = 'Tornado',
    Package = 'Package',
    Health = 'Health',
    Protest = 'Protest',
    VIP = 'VIP',
    Pollution = 'Pollution',
}

export enum ActionPlanType {
    None,
    Notification = 'notification',
    Email = 'email',
    RapidSOS = 'rapidsos',
    LightSensor = 'lightsensor',
}

export enum ActionStatus {
    None = '',
    Unknown = 'Unknown',
    Success = 'Success',
    Skipped = 'Skipped',
    NotStarted = 'NotStarted',
    Error = 'Error'
}

export interface ActionPlanAction {
    actionId?: string;
    actionType: ActionPlanType;
    isActive: true;
    description?: string;
    parameters?: any;
    endDate?: Date;
    startDate?: Date;
    status?: ActionStatus;
}

export enum ActionChangeType {
    Add = 'add',
    Edit = 'edit',
    Delete = 'delete'
}

export interface AddEditAction {
    isRemoveAction?: boolean;
    isCloseAction?: boolean;
    actionChangedString?: ActionChangeType
    action?: ActionPlanAction,
}

export interface ActionPlanTextAction extends ActionPlanAction {
    parameters: {
        content: string
    }
}

export interface ActionPlanNotificationAction extends ActionPlanAction {
    parameters: {
        message: string,
        editing?: boolean
    }
}

export interface ActionPlanRadiusAction extends ActionPlanAction {
    parameters: {
        radius: string
        color: string
    }
}

export type ActionPlanActionTypes =
    | ActionPlanAction
    | ActionPlanNotificationAction
    | ActionPlanRadiusAction
    | ActionPlanTextAction

export interface ActionPlan {
    actionPlanId: string;
    name: string;
    description: string;
    isActive: true;
    color: ActionPlanColor;
    icon: ActionPlanIcon;
    openActions?: ActionPlanActionTypes[];
    closeActions?: ActionPlanActionTypes[];
    primaryRadius: number;
    secondaryRadius: number;
    acceptSafeStatus: boolean;
}
