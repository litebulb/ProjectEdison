import {
    ActionPlan, ActionPlanColor, ActionPlanIcon, ActionPlanType, ActionStatus
} from '../reducers/action-plan/action-plan.model';

export const mockActionPlans: ActionPlan[] = [
    {
        actionPlanId: '1',
        name: 'Test',
        description: 'Testing Plan',
        isActive: true,
        acceptSafeStatus: true,
        primaryRadius: 1,
        secondaryRadius: 2,
        color: ActionPlanColor.Red,
        icon: ActionPlanIcon.Fire,
        openActions: [
            {
                actionId: '1',
                actionType: ActionPlanType.EmergencyCall,
                isActive: true,
                description: '911 will be called by Twilio',
                status: ActionStatus.None,
            },
            {
                actionId: '2',
                actionType: ActionPlanType.Notification,
                isActive: true,
                description: 'Mobile App Notification sent to 2,134 people',
                parameters: {
                    content: `A potential fire has been detected near the Performing Arts Center on the
             Southeast Side of campus. If in the vicinity, leave immediately via the nearest fire exit.`,
                },
                status: ActionStatus.None,
            },
            {
                actionId: '3',
                actionType: ActionPlanType.LightSensor,
                isActive: true,
                description: 'Primary',
                parameters: {
                    color: 'RED',
                },
                status: ActionStatus.None,
            },
            {
                actionId: '4',
                actionType: ActionPlanType.LightSensor,
                isActive: true,
                description: 'Secondary',
                parameters: {
                    color: 'YELLOW',
                },
                status: ActionStatus.None,
            },
        ],
        closeActions: [
            {
                actionId: '1',
                actionType: ActionPlanType.EmergencyCall,
                isActive: true,
                description: '911 will be called by Twilio',
                status: ActionStatus.None,
            },
            {
                actionId: '2',
                actionType: ActionPlanType.Notification,
                isActive: true,
                description: 'Mobile App Notification sent to 2,134 people',
                parameters: {
                    content: `A potential fire has been detected near the Performing Arts Center on the
             Southeast Side of campus. If in the vicinity, leave immediately via the nearest fire exit.`,
                },
                status: ActionStatus.None,
            },
            {
                actionId: '3',
                actionType: ActionPlanType.LightSensor,
                isActive: true,
                description: 'Primary',
                parameters: {
                    color: 'RED',
                },
                status: ActionStatus.None,
            },
            {
                actionId: '4',
                actionType: ActionPlanType.LightSensor,
                isActive: true,
                description: 'Secondary',
                parameters: {
                    color: 'YELLOW',
                },
                status: ActionStatus.None,
            },
        ],
    }
]
