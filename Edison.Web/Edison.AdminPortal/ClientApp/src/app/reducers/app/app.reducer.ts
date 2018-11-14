import { AppActionTypes, AppActions } from './app.actions'

export interface State {
  pageTitle: string
}

export const initialState: State = {
  pageTitle: null,
}

export function reducer(state = initialState, action: AppActions): State {
  switch (action.type) {
    case AppActionTypes.UpdatePageTitle:
      return {
        ...state,
        pageTitle: action.payload.title,
      }
    default:
      return state
  }
}
