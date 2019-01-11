import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

import {
    ActionPlanTextAction, ActionStatus
} from '../../../../../../reducers/action-plan/action-plan.model';

@Component({
    selector: 'app-text-template',
    templateUrl: './text-template.component.html',
    styleUrls: [ './text-template.component.scss' ],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class TextTemplateComponent {
    @Input() context: ActionPlanTextAction;
    @Input() last: boolean;

    actionStatus = ActionStatus;
}
