import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Response } from '../../../../reducers/response/response.model';

@Component({
  selector: 'app-active-response-icon',
  templateUrl: './active-response-icon.component.html',
  styleUrls: ['./active-response-icon.component.scss']
})
export class ActiveResponseIconComponent implements OnInit {
  @Input() response: Response;
  @Output() click = new EventEmitter();
  iconClass: string;

  constructor() { }

  ngOnInit() {
    this.iconClass = `${this.response.icon.toLowerCase()}-static ${this.response.color.toLowerCase()}`;
  }

  onClick(event: Event) {
    event.stopPropagation();
    this.click.emit();
  }

}
