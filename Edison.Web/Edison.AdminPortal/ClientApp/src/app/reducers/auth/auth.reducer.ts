import { Action } from '@ngrx/store';
import { AuthActions, AuthActionTypes } from './auth.actions';
import { User } from 'msal';


export interface State {
  token: string;
  user: User;
}

export const initialState: State = {
  token: null,
  user: null,
};

export function reducer(state = initialState, action: AuthActions): State {
  switch (action.type) {

    case AuthActionTypes.SetToken: {
      return {
        ...state,
        token: action.payload.token,
      };
    }

    case AuthActionTypes.SetUser: {
      return {
        ...state,
        user: action.payload.user,
      };
    }

    case AuthActionTypes.LogOut: {
      return {
        ...state,
        user: null,
        token: null,
      };
    }

    default:
      return state;
  }
}
