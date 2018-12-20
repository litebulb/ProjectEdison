import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import { ActionPlan } from '../../../../reducers/action-plan/action-plan.model';

@Component({
    selector: 'app-action-plan-list',
    templateUrl: './action-plan-list.component.html',
    styleUrls: [ './action-plan-list.component.scss' ]
})
export class ActionPlanListComponent implements OnInit {
    @Input() actionPlans: ActionPlan[];

    @Output() actionPlanChanged = new EventEmitter<ActionPlan>();

    constructor () { }

    ngOnInit() {
    }

    onActionPlanChanged(actionPlan: ActionPlan) {
        this.actionPlanChanged.emit(actionPlan);
    }

}
