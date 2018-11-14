import { AppActionTypes, AppActions } from './app.actions'

export interface State {
    title: string;
    sidebar: boolean;
}

export const initialState: State = {
    title: null,
    sidebar: null,
}

export function reducer(state = initialState, action: AppActions): State {
    switch (action.type) {
        case AppActionTypes.UpdatePageData:
            return {
                ...state,
                ...action.payload
            }
        default:
            return state
    }
}
