import { Component, Input, Output, EventEmitter, OnInit, OnChanges } from '@angular/core';
import { Response, ResponseState } from '../../../../reducers/response/response.model';
import { Store } from '@ngrx/store';
import { AppState } from '../../../../reducers';
import { CloseResponse } from '../../../../reducers/response/response.actions';
import { SetSelectingActionPlan } from '../../../../reducers/action-plan/action-plan.actions';

@Component({
    selector: 'app-deactivate-response',
    templateUrl: './deactivate-response.component.html',
    styleUrls: [ './deactivate-response.component.scss' ],
})
export class DeactivateResponseComponent implements OnInit, OnChanges {
    @Input() activeResponse: Response;
    @Output() deactivateClick = new EventEmitter();
    @Output() backClick = new EventEmitter();
    @Output() returnToMapClick = new EventEmitter();

    deactivated: boolean;

    constructor (private store: Store<AppState>) { }

    ngOnInit() {
        this.updateState();
    }

    ngOnChanges() {
        this.updateState();
    }

    updateState() {
        this.deactivated = this.activeResponse.responseState === 0;
    }

    onBackClick() {
        this.backClick.emit();
    }

    onReturnToMapClick() {
        this.returnToMapClick.emit();
        this.store.dispatch(new SetSelectingActionPlan({ isSelecting: false }));
    }

    onDeactivateClick() {
        this.store.dispatch(new CloseResponse({ responseId: this.activeResponse.responseId, state: ResponseState.Inactive }));
    }

}
