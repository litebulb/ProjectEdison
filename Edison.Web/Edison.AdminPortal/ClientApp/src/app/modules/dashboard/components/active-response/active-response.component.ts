import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { Response } from '../../../../reducers/response/response.model';

@Component({
  selector: 'app-active-response',
  templateUrl: './active-response.component.html',
  styleUrls: ['./active-response.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ActiveResponseComponent {
  @Input() activeResponse: Response;
  @Output() deactivateClicked = new EventEmitter();

  deactivate() {
    this.deactivateClicked.emit();
  }
}
