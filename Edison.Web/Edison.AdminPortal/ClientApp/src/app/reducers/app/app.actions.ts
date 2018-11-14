import { Action } from '@ngrx/store'

export enum AppActionTypes {
  FocusAllPins = '[App] Focus All Pins',
  UpdatePageTitle = '[App] Update Page Title',
}

export class FocusAllPins implements Action {
  readonly type = AppActionTypes.FocusAllPins
}

export class UpdatePageTitle implements Action {
  readonly type = AppActionTypes.UpdatePageTitle

  constructor(public payload: { title: string }) {}
}

export type AppActions = FocusAllPins | UpdatePageTitle
