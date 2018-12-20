import { AppActions, AppActionTypes } from './app.actions';

export interface State {
    title: string;
    showDownArrow: boolean;
    showReloadButton: boolean;
}

export const initialState: State = {
    title: null,
    showDownArrow: null,
    showReloadButton: null,
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
