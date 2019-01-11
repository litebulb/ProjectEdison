import { Observable } from 'rxjs';

import { Component, HostListener, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material';
import { select, Store } from '@ngrx/store';

import { AppState } from '../../../../reducers';
import {
    GetActionPlan, GetActionPlans, PutActionPlan
} from '../../../../reducers/action-plan/action-plan.actions';
import { ActionPlan } from '../../../../reducers/action-plan/action-plan.model';
import { actionPlansSelector } from '../../../../reducers/action-plan/action-plan.selectors';
import {
    ConfirmDialogComponent
} from '../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
    selector: 'app-responses',
    templateUrl: './responses.component.html',
    styleUrls: [ './responses.component.scss' ]
})
export class ResponsesComponent implements OnInit {
    actionPlans$: Observable<ActionPlan[]>;
    saveDisabled: boolean = true;
    actionPlansToUpdate: any = {};

    constructor (private store: Store<AppState>, public dialog: MatDialog) { }

    ngOnInit() {
        this.actionPlans$ = this.store.pipe(select(actionPlansSelector));

        this.store.dispatch(new GetActionPlans());
    }

    canDeactivate() {
        if (!this.saveDisabled) {
            const dialogRef = this.dialog.open(ConfirmDialogComponent, {
                width: '460px',
                data: {
                    defaultText: 'You have unsaved changes that will be lost. Do you want to continue?',
                }
            });
            return dialogRef.afterClosed();
        }
        return true;
    }

    actionPlanChanged(actionPlan: ActionPlan) {
        this.saveDisabled = false;
        this.actionPlansToUpdate[ actionPlan.actionPlanId ] = actionPlan;
    }

    saveChanges() {
        if (!this.saveDisabled) {
            Object.values(this.actionPlansToUpdate).forEach((actionPlan: ActionPlan) => {
                this.store.dispatch(new PutActionPlan({ actionPlan }));
            });
            this.actionPlansToUpdate = {}; // reset action plans to update
            this.saveDisabled = true; // disable save button
        }
    }

    getActionPlanDetails(actionPlanId: string) {
        this.store.dispatch(new GetActionPlan({ actionPlanId }));
    }

}
