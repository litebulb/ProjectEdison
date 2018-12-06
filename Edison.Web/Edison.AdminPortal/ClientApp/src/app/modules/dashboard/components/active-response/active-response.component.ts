import {
    ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output
} from '@angular/core';
import { Store } from '@ngrx/store';

import { AppState } from '../../../../reducers';
import { ShowSelectingLocation } from '../../../../reducers/response/response.actions';
import { Response } from '../../../../reducers/response/response.model';

@Component({
    selector: 'app-active-response',
    templateUrl: './active-response.component.html',
    styleUrls: [ './active-response.component.scss' ],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ActiveResponseComponent implements OnInit {
    @Input() activeResponse: Response;
    @Output() deactivateClicked = new EventEmitter();
    @Output() updateClicked = new EventEmitter();
    @Output() setLocationClicked = new EventEmitter();

    constructor (private store: Store<AppState>) { }

    responseHasLocation: boolean = false;

    ngOnInit() {
        if (this.activeResponse.primaryEventClusterId === null &&
            this.activeResponse.geolocation) {
            this.responseHasLocation = true;
        } else {
            this.responseHasLocation = false;
        }
    }

    deactivate() {
        this.deactivateClicked.emit();
    }

    update() {
        this.updateClicked.emit();
    }

    setLocation() {
        this.setLocationClicked.emit();
        this.store.dispatch(new ShowSelectingLocation({
            showSelectingLocation: true,
            response: this.activeResponse
        }));
    }
}
