import { Injectable } from '@angular/core';
import { CanActivate, Router, CanLoad } from '@angular/router';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate, CanLoad {

  constructor(
    private auth: AuthService,
  ) { }

  canLoad(): boolean {
    return this.verifyAuth();
  }

  canActivate(): boolean {
    return this.verifyAuth();
  }

  private verifyAuth() {
    if (!this.auth.loggedIn() && environment.authorize) {
      if (!this.auth.loginInProgress() && !this.auth.loggedIn()) {
        this.auth.login();
        return false;
      }
    }

    if (!this.auth.loginInProgress() && environment.authorize) {
      this.auth.refreshUser();
      this.auth.refreshToken();
    }

    return true;
  }
}
