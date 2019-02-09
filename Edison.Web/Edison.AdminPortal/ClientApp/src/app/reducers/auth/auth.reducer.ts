import { User } from 'msal';

import { AuthActions, AuthActionTypes } from './auth.actions';
import { UserRole } from 'src/app/core/services/directline/models/user-model';

export interface State {
  token: string;
  user: {
    userName: string;
    profile: {
      email: string;
    };
    id: string;
    name?: string;
    iconUrl?: string;
    role?: UserRole;
  };
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
