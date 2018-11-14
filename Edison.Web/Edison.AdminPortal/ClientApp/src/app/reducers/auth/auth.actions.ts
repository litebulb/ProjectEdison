import { Action } from '@ngrx/store';

export enum AuthActionTypes {
  SetToken = '[Auth] Set Token',
  SetUser = '[Auth] Set User',
  LogOut = '[Auth] Log Out',
}

export class SetToken implements Action {
    readonly type = AuthActionTypes.SetToken;

    constructor(public payload: { token: string }) {}
}

export class SetUser implements Action {
    readonly type = AuthActionTypes.SetUser;

    constructor(public payload: { user: any }) {}
}

export class LogOut implements Action {
    readonly type = AuthActionTypes.LogOut;
}

export type AuthActions =
 SetToken
 | SetUser
 | LogOut;
