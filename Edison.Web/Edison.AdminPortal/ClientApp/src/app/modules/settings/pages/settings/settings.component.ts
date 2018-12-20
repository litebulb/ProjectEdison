import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';

import { AppState } from '../../../../reducers';
import { AppPage, SetPageData } from '../../../../reducers/app/app.actions';

@Component({
    selector: 'app-settings',
    templateUrl: './settings.component.html',
    styleUrls: [ './settings.component.scss' ]
})
export class SettingsComponent implements OnInit {

    constructor (private store: Store<AppState>) { }

    ngOnInit() {
        this.store.dispatch(new SetPageData({ title: AppPage.Settings, showDownArrow: true, showReloadButton: false }))
    }

}
