import { Subject } from 'rxjs';

import {
    Component, ComponentFactoryResolver, ComponentRef, Input, OnDestroy, OnInit, ViewChild,
    ViewContainerRef
} from '@angular/core';

import { fadeMSeconds } from '../../../../core/animations/fadeInOut';
import {
    ActionChangeType, ActionPlanActionTypes, ActionPlanType, AddEditAction
} from '../../../../reducers/action-plan/action-plan.model';
import {
    NotificationTemplateComponent
} from './templates/notification-template/notification-template.component';
import { RadiusTemplateComponent } from './templates/radius-template/radius-template.component';
import { TextTemplateComponent } from './templates/text-template/text-template.component';

@Component({
    selector: 'app-action-list-item',
    templateUrl: './action-list-item.component.html',
    styleUrls: [ './action-list-item.component.scss' ],
})
export class ActionListItemComponent implements OnInit, OnDestroy {
    @ViewChild('container', { read: ViewContainerRef }) container: ViewContainerRef;

    @Input() context: ActionPlanActionTypes;
    @Input() last: boolean;
    @Input() first: boolean;
    @Input() canEdit: boolean;
    @Input() isCloseAction: boolean;
    @Input() actionChangeSubject: Subject<{ addEditAction: AddEditAction, actionChangeType: ActionChangeType }>;

    private componentRef: ComponentRef<{}>

    constructor (private componentFactoryResolver: ComponentFactoryResolver) { }

    getComponentType(type: ActionPlanType): any {
        switch (type) {
            case ActionPlanType.EmergencyCall:
            case ActionPlanType.Email:
            case ActionPlanType.Twilio:
                return TextTemplateComponent;
            case ActionPlanType.Notification:
                return NotificationTemplateComponent;
            case ActionPlanType.LightSensor:
                return RadiusTemplateComponent;
            default:
                return TextTemplateComponent;
        }
    }

    ngOnInit() {
        if (this.context.actionType) {
            const componentType = this.getComponentType(this.context.actionType);

            // note: componentType must be declared within module.entryComponents
            const factory = this.componentFactoryResolver.resolveComponentFactory(componentType);
            this.componentRef = this.container.createComponent(factory);

            // set component context
            const instance = <ActionListItemContext> this.componentRef.instance;
            instance.context = this.context;
            instance.last = this.last;
            instance.first = this.first;
            instance.canEdit = this.canEdit;
            instance.actionChangeSubject = this.actionChangeSubject;
            instance.isCloseAction = this.isCloseAction;
        }
    }

    ngOnDestroy() {
        if (this.componentRef) {
            setTimeout(() => {
                this.componentRef.destroy();
                this.componentRef = null;
            }, fadeMSeconds);
        }
    }
}

export abstract class ActionListItemContext {
    context: ActionPlanActionTypes;
    last: boolean;
    first: boolean;
    canEdit: boolean;
    actionChangeSubject: Subject<{ addEditAction: AddEditAction, actionChangeType: ActionChangeType }>;
    isCloseAction: boolean;
}
