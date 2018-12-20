import { Observable } from 'rxjs';

import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { select, Store } from '@ngrx/store';

import { AdalService } from '../../../../core/services/adal.service';
import { AppState } from '../../../../reducers';
import { FocusAllPins } from '../../../../reducers/app/app.actions';
import { State } from '../../../../reducers/app/app.reducer';
import { appStateSelector, pageTitleSelector } from '../../../../reducers/app/app.selectors';

@Component({
    selector: 'app-topnav',
    templateUrl: './topnav.component.html',
    styleUrls: [ './topnav.component.scss' ],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TopnavComponent implements OnInit {
    pageTitle$: Observable<string>;
    appState$: Observable<State>;

    constructor (
        private authService: AdalService,
        private store: Store<AppState>
    ) { }

    ngOnInit() {
        this.pageTitle$ = this.store.pipe(select(pageTitleSelector));
        this.appState$ = this.store.pipe(select(appStateSelector));
    }

    reload() {
        this.store.dispatch(new FocusAllPins());
    }

    logout() {
        this.authService.logout();
    }
}
