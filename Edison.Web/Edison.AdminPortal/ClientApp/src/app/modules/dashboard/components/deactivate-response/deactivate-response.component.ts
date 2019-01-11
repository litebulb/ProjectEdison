import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';

import { Response, ResponseState } from '../../../../reducers/response/response.model';

@Component({
    selector: 'app-deactivate-response',
    templateUrl: './deactivate-response.component.html',
    styleUrls: [ './deactivate-response.component.scss' ],
})
export class DeactivateResponseComponent implements OnChanges {
    @Input() activeResponse: Response;
    @Output() deactivateClick = new EventEmitter();
    @Output() backClick = new EventEmitter();
    @Output() returnToMapClick = new EventEmitter();

    deactivated: boolean;
    disabled: boolean;

    constructor () { }

    ngOnChanges() {
        if (this.activeResponse) { this.deactivated = this.activeResponse.responseState === ResponseState.Inactive; }
    }

    onBackClick() { this.backClick.emit(); }

    onReturnToMapClick() { this.returnToMapClick.emit(); }

    onDeactivateClick() {
        this.disabled = true;
        this.deactivateClick.emit(this.activeResponse.responseId);
    }

}
