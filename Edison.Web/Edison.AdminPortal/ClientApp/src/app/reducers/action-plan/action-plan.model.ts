export enum ActionPlanColor {
    Red = 'Red',
    Green = 'Green',
    Yellow = 'Yellow',
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

export enum ActionPlanStatus {
    InComplete,
    Complete,
}

export interface ActionPlanAction {
    actionId?: string
    actionType: ActionPlanType
    isActive: true
    description?: string
    parameters?: any
    status?: ActionPlanStatus
}

export enum ActionChangeType {
    Add = 'add',
    Edit = 'edit',
    Delete = 'delete'
}

export interface AddEditAction {
    isCloseAction: boolean;
    actionChangedString: ActionChangeType
    action: ActionPlanAction,
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
    | ActionPlanNotificationAction
    | ActionPlanRadiusAction
    | ActionPlanTextAction

export interface ActionPlan {
    actionPlanId: string
    name: string
    description: string
    isActive: true
    acceptSafeStatus: boolean
    primaryRadius: number
    secondaryRadius: number
    color: ActionPlanColor
    icon: ActionPlanIcon
    openActions?: ActionPlanAction[]
    closeActions?: ActionPlanAction[]
}
