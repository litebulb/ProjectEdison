import { Component, Input, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ActionPlanRadiusAction } from '../../../../../../reducers/action-plan/action-plan.model';

@Component({
  selector: 'app-radius-template',
  templateUrl: './radius-template.component.html',
  styleUrls: ['./radius-template.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RadiusTemplateComponent implements OnInit {
  @Input()
  context: ActionPlanRadiusAction;

  @Input()
  last: boolean;

  ngOnInit() {
  }

  getBgColor() {
    return this.context.parameters.color.toLowerCase();
  }
}
