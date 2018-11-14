import { Component, ChangeDetectionStrategy, OnInit } from '@angular/core'
import { Store, select } from '@ngrx/store'
import { AppState } from '../../../../reducers'
import { FocusAllPins } from '../../../../reducers/app/app.actions'
import { Observable } from 'rxjs'
import { pageTitleSelector, pageSidebarSelector } from '../../../../reducers/app/app.selectors'
import { AdalService } from '../../../../core/services/adal.service';

@Component({
    selector: 'app-topnav',
    templateUrl: './topnav.component.html',
    styleUrls: [ './topnav.component.scss' ],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TopnavComponent implements OnInit {
    pageTitle$: Observable<string>;
    sidebar$: Observable<boolean>;

    constructor (
        private authService: AdalService,
        private store: Store<AppState>
    ) { }

    ngOnInit() {
        this.pageTitle$ = this.store.pipe(select(pageTitleSelector));
        this.sidebar$ = this.store.pipe(select(pageSidebarSelector));
    }

    reload() {
        this.store.dispatch(new FocusAllPins());
    }

    logout() {
        this.authService.logout();
    }
}
