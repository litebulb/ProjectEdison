import { Subscription } from 'rxjs';

import { Component, OnDestroy, OnInit } from '@angular/core';
import { select, Store } from '@ngrx/store';

import { environment } from '../../../../../environments/environment';
import { AppState } from '../../../../reducers';
import { AppPage, SetPageData } from '../../../../reducers/app/app.actions';
import { authTokenSelector } from '../../../../reducers/auth/auth.selectors';

@Component({
    selector: 'app-history',
    templateUrl: './history.component.html',
    styleUrls: [ './history.component.scss' ]
})
export class HistoryComponent implements OnInit, OnDestroy {
    downloadDisabled: boolean = true;
    selectedRange: { startDate: Date, endDate: Date };
    downloadLink: string;
    token: string;
    tokenSub: Subscription;

    constructor (private store: Store<AppState>) { }

    ngOnInit() {
        this.store.dispatch(new SetPageData({ title: AppPage.History, showDownArrow: true, showReloadButton: true }));
        this.tokenSub = this.store.pipe(select(authTokenSelector)).subscribe(token => this.token = token);
    }

    ngOnDestroy() {
        this.tokenSub.unsubscribe();
    }

    rangeSelected(range: { startDate: Date, endDate: Date }) {
        this.selectedRange = range;
        const { startDate, endDate } = range;

        let paramStrings = `?minimumDate=${startDate.toISOString()}`;
        paramStrings += `&maximumDate=${endDate.toISOString()}`;

        const fileName = `Event & Response History - ${this._formatDateString(startDate)} - ${this._formatDateString(endDate)}`;

        paramStrings += `&filename=${encodeURIComponent(fileName)}`;
        paramStrings += `&access_token=${this.token}`;

        this.downloadLink = `${environment.baseUrl}${environment.apiUrl}reports${paramStrings}`;
    }

    validRangeSelected(valid: boolean) {
        this.downloadDisabled = !valid;
    }

    checkLinkValid() {
        if (this.downloadDisabled) {
            return false;
        }

        return true;
    }

    private _formatDateString(date: Date) {
        const dayStr = date.getDate().toString().padStart(2, '0');
        const monthStr = date.getMonth().toString().padStart(2, '0');
        const yearStr = date.getFullYear().toString().substr(2);

        return `${dayStr}.${monthStr}.${yearStr}`;
    }

}
