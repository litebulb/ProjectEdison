import { Component, Input, OnChanges, OnDestroy } from '@angular/core';
import { ActionPlanAction } from '../../../../reducers/action-plan/action-plan.model';
import { listFadeInOut } from '../../../../shared/animations/listFadeInOut';
import { fadeInOutBorder } from '../../../../shared/animations/fadeInOutBorder';

@Component({
  selector: 'app-action-list',
  templateUrl: './action-list.component.html',
  styleUrls: ['./action-list.component.scss'],
  animations: [
    listFadeInOut,
    fadeInOutBorder,
  ],
})
export class ActionListComponent {
  @Input() actions: ActionPlanAction[];

  constructor() {
  }
}
