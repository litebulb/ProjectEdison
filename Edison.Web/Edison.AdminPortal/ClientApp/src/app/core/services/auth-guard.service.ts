import { Injectable } from '@angular/core';
import { CanActivate, Router, CanLoad } from '@angular/router';
import { MsalService } from './msal.service';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class AuthGuardService implements CanActivate, CanLoad {

    constructor (
        private auth: MsalService,
    ) { }

    canLoad(): boolean {
        return this.verifyAuth();
    }

    canActivate(): boolean {
        return this.verifyAuth();
    }

    private verifyAuth() {
        if (environment.authorize) {
            if (!this.auth.loggedIn()) {
                if (!this.auth.loginInProgress() && !this.auth.loggedIn()) {
                    this.auth.login();
                    return false;
                }
            }

            if (!this.auth.loginInProgress() && environment.authorize) {
                this.auth.refreshUser();
                this.auth.refreshToken();
            }
        }

        return true;
    }
}
