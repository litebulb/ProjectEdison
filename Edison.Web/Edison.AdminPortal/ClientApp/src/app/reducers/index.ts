import { ActionReducerMap, MetaReducer } from '@ngrx/store'
import { environment } from '../../environments/environment'
import * as fromRouterStore from '@ngrx/router-store'
import * as fromEventStore from './event/event.reducer'
import * as fromDeviceStore from './device/device.reducer'
import * as fromAuthStore from './auth/auth.reducer'
import * as fromResponseStore from './response/response.reducer'
import * as fromActionPlanStore from './action-plan/action-plan.reducer'
import * as fromAppStore from './app/app.reducer'
import * as fromChatStore from './chat/chat.reducer';

export interface AppState {
    router: fromRouterStore.RouterReducerState
    event: fromEventStore.State
    device: fromDeviceStore.State
    auth: fromAuthStore.State
    response: fromResponseStore.State
    actionPlan: fromActionPlanStore.State
    app: fromAppStore.State
    chat: fromChatStore.State
}

export const reducers: ActionReducerMap<AppState> = {
    router: fromRouterStore.routerReducer,
    event: fromEventStore.reducer,
    device: fromDeviceStore.reducer,
    auth: fromAuthStore.reducer,
    response: fromResponseStore.reducer,
    actionPlan: fromActionPlanStore.reducer,
    app: fromAppStore.reducer,
    chat: fromChatStore.reducer
}

export const metaReducers: MetaReducer<AppState>[] = !environment.production
    ? []
    : []
