import { Action } from '@ngrx/store'

export enum AppActionTypes {
    FocusAllPins = '[App] Focus All Pins',
    UpdatePageData = '[App] Update Page Data',
}

export class FocusAllPins implements Action {
    readonly type = AppActionTypes.FocusAllPins
}

export class SetPageData implements Action {
    readonly type = AppActionTypes.UpdatePageData

    constructor (public payload: { title: string, sidebar?: boolean }) { }
}

export type AppActions = FocusAllPins | SetPageData
