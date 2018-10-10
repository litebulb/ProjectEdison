import { Component, OnInit, Input, ChangeDetectionStrategy } from '@angular/core';
import { ActionPlanNotificationAction } from '../../../../../../reducers/action-plan/action-plan.model';

@Component({
  selector: 'app-notification-template',
  templateUrl: './notification-template.component.html',
  styleUrls: ['./notification-template.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NotificationTemplateComponent implements OnInit {
  @Input()
  context: ActionPlanNotificationAction;

  @Input()
  last: boolean;

  constructor() { }

  ngOnInit() {
  }

}
