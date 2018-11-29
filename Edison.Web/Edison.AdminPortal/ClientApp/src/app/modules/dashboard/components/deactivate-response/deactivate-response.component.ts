import { Component, EventEmitter, Input, OnChanges, OnInit, Output } from '@angular/core';
import { Store } from '@ngrx/store';

import { AppState } from '../../../../reducers';
import { SetSelectingActionPlan } from '../../../../reducers/action-plan/action-plan.actions';
import { CloseResponse } from '../../../../reducers/response/response.actions';
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

    constructor (private store: Store<AppState>) { }

    ngOnChanges() {
        if (this.activeResponse) {
            this.deactivated = this.activeResponse.responseState === ResponseState.Inactive;
        }
    }

    onBackClick() {
        this.backClick.emit();
    }

    onReturnToMapClick() {
        this.returnToMapClick.emit();
        this.store.dispatch(new SetSelectingActionPlan({ isSelecting: false }));
    }

    onDeactivateClick() {
        this.disabled = true;
        this.store.dispatch(new CloseResponse({ responseId: this.activeResponse.responseId, state: ResponseState.Inactive }));
    }

}
