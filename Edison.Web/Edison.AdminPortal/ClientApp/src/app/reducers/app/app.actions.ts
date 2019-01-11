import { Action } from '@ngrx/store';

export enum AppPage {
    RightNow = 'RIGHT NOW',
    Devices = 'DEVICES',
    Messaging = 'MESSAGING',
    History = 'HISTORY',
    Settings = 'SETTINGS',
}

export enum AppActionTypes {
    FocusAllPins = '[App] Focus All Pins',
    UpdatePageData = '[App] Update Page Data',
    ToggleOverlay = '[App] Show Overlay'
}

export class FocusAllPins implements Action {
    readonly type = AppActionTypes.FocusAllPins
}

export class SetPageData implements Action {
    readonly type = AppActionTypes.UpdatePageData

    constructor (public payload: { title: AppPage, showDownArrow?: boolean, showReloadButton?: boolean }) { }
}

export class ToggleOverlay implements Action {
    readonly type = AppActionTypes.ToggleOverlay;

    constructor(public payload: { state?: boolean }) {}
}

export type AppActions = FocusAllPins | SetPageData
