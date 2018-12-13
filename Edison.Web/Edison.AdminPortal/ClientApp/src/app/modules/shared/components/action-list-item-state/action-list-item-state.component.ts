import { Component, Input, OnInit } from '@angular/core';

import { ActionStatus } from '../../../../reducers/action-plan/action-plan.model';

@Component({
    selector: 'app-action-list-item-state',
    templateUrl: './action-list-item-state.component.html',
    styleUrls: [ './action-list-item-state.component.scss' ]
})
export class ActionListItemStateComponent implements OnInit {
    @Input() status: ActionStatus;
    @Input() loading: boolean;

    actionStatus = ActionStatus;

    constructor () { }

    ngOnInit() {
    }

}
