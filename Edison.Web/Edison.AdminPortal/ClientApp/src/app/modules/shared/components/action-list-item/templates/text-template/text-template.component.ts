import { Component, OnInit, Input, ChangeDetectionStrategy } from '@angular/core';
import { ActionPlanTextAction } from '../../../../../../reducers/action-plan/action-plan.model';

@Component({
    selector: 'app-text-template',
    templateUrl: './text-template.component.html',
    styleUrls: [ './text-template.component.scss' ],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class TextTemplateComponent implements OnInit {
    @Input()
    context: ActionPlanTextAction;

    @Input()
    last: boolean;

    ngOnInit() {
    }

}
