import { Injectable } from '@angular/core';
import * as Msal from 'msal';
import { environment } from '../../../environments/environment';
import { Store } from '@ngrx/store';
import { AppState } from '../../reducers';
import { SetUser, SetToken, LogOut } from '../../reducers/auth/auth.actions';
import { Router } from '@angular/router';

@Injectable({
    providedIn: 'root'
})
export class MsalService {
    private config = {
        clientID: environment.azureAd.clientId,
        graphScopes: [ 'openid' ],
        authority: environment.azureAd.authority,
        tenant: environment.azureAd.tenant,
    };

    private app: Msal.UserAgentApplication;

    constructor (private store: Store<AppState>, private router: Router) {
        this.app = new Msal.UserAgentApplication(this.config.clientID, `${this.config.authority}${this.config.tenant}`, () => { }, {
            redirectUri: location.origin,
            postLogoutRedirectUri: location.origin
        });
    }

    public login() {
        return this.app.loginRedirect(this.config.graphScopes, '');
    }

    public loginInProgress() {
        return this.app.loginInProgress();
    }

    public logout() {
        this.store.dispatch(new LogOut());
        this.app.logout();
    }

    public refreshUser() {
        const user = this.app.getUser();

        if (user) {
            this.store.dispatch(new SetUser({ user }));
        }
    }

    public refreshToken() {
        return this.app.acquireTokenSilent(this.config.graphScopes)
            .then(token => this.store.dispatch(new SetToken({ token })),
                error => {
                    return this.app.loginRedirect(this.config.graphScopes);
                });
    }

    public loggedIn() {
        return environment.authorize ? this.app.getUser() !== null : true;
    }
}
