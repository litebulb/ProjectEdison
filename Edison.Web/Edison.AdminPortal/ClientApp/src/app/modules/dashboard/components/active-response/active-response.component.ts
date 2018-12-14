import {
    ChangeDetectionStrategy, Component, EventEmitter, Input, OnChanges, OnInit, Output
} from '@angular/core';
import { Store } from '@ngrx/store';

import { AppState } from '../../../../reducers';
import { ActionStatus } from '../../../../reducers/action-plan/action-plan.model';
import {
    RetryResponseActions, ShowSelectingLocation
} from '../../../../reducers/response/response.actions';
import { Response } from '../../../../reducers/response/response.model';

@Component({
    selector: 'app-active-response',
    templateUrl: './active-response.component.html',
    styleUrls: [ './active-response.component.scss' ],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ActiveResponseComponent implements OnInit, OnChanges {
    @Input() activeResponse: Response;
    @Output() deactivateClicked = new EventEmitter();
    @Output() updateClicked = new EventEmitter();
    @Output() setLocationClicked = new EventEmitter();

    constructor (private store: Store<AppState>) { }

    responseHasLocation: boolean = false;
    actionPlanSuccessful: boolean;
    actionPlanNeedsLocation: boolean;
    responseNeedsLocation: boolean;
    actionPlanHasErrors: boolean;
    actionPlanPartialSuccess: boolean;

    ngOnInit() {
        if (this.activeResponse.geolocation) {
            this.responseHasLocation = true;
        } else {
            this.responseHasLocation = false;
        }
        this.updateResponseStatus();
    }

    ngOnChanges() {
        this.updateResponseStatus();
    }

    deactivate() {
        this.deactivateClicked.emit();
    }

    update() {
        this.updateClicked.emit();
    }

    updateResponseStatus() {
        if (this.activeResponse && this.activeResponse.actionPlan && this.activeResponse.actionPlan.openActions) {
            this.actionPlanSuccessful = !this.activeResponse.actionPlan.openActions.some(action => action.status !== ActionStatus.Success);
            this.actionPlanNeedsLocation = this.responseNeedsLocation && this.activeResponse.actionPlan.openActions.some(action => action.status === ActionStatus.Skipped);
            this.actionPlanHasErrors = this.activeResponse.actionPlan.openActions.some(action => action.status === ActionStatus.Error || action.status === ActionStatus.Unknown);
            this.actionPlanPartialSuccess = this.activeResponse.actionPlan.openActions.some(action => action.status === ActionStatus.Success);
        } else {
            this.actionPlanSuccessful = false;
            this.actionPlanNeedsLocation = false;
            this.actionPlanHasErrors = false;
            this.actionPlanPartialSuccess = false;
        }
    }

    retry() {
        if (this.activeResponse) {
            this.store.dispatch(new RetryResponseActions({ responseId: this.activeResponse.responseId }));
        }
    }

    setLocation() {
        this.setLocationClicked.emit();
        this.store.dispatch(new ShowSelectingLocation({
            showSelectingLocation: true,
            response: this.activeResponse
        }));
    }
}
