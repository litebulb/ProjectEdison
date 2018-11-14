import { Injectable, OnDestroy } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from '../../reducers';
import { LogOut, SetToken, SetUser } from '../../reducers/auth/auth.actions';
import { MsAdalAngular6Service } from 'microsoft-adal-angular6';
import { environment } from '../../../environments/environment';
import { Subscription } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AdalService implements OnDestroy {
    private _tokenSub: Subscription;
    private _token: string;

    constructor (private store: Store<AppState>,
        private adalSvc: MsAdalAngular6Service) {
        this._tokenSub = adalSvc.acquireToken(environment.baseUrl)
            .subscribe(token => {
                this._token = token;
                this.store.dispatch(new SetToken({ token }));
                this.store.dispatch(new SetUser({ user: this.adalSvc.userInfo }))
            });
    }

    ngOnDestroy() {
        this._tokenSub.unsubscribe();
    }

    public login() {
        return this.adalSvc.login();
    }

    public logout() {
        this.store.dispatch(new LogOut());
        this.adalSvc.logout();
    }

    public loggedIn() {
        return this.adalSvc.isAuthenticated !== null;
    }
}
