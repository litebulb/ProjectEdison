import { BrowserModule, DomSanitizer } from '@angular/platform-browser'
import { NgModule } from '@angular/core'
import { StoreRouterConnectingModule } from '@ngrx/router-store'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { StoreModule } from '@ngrx/store'
import { StoreDevtoolsModule } from '@ngrx/store-devtools'
import { EffectsModule } from '@ngrx/effects'
import 'hammerjs'
import { MsAdalAngular6Module, AuthenticationGuard } from 'microsoft-adal-angular6';

import { AppRoutingModule } from './app-routing.module'
import { environment } from '../environments/environment'
import { reducers, metaReducers } from './reducers'
import { MaterialModule } from './modules/material/material.module'
import { AppComponent } from './app.component'
import { SharedModule } from './modules/shared/shared.module'
import { MsalService } from './core/services/msal.service'
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http'
import { effects } from './effects'
import { SignalRService } from './core/services/signal-r.service'
import { DirectlineService } from './core/services/directline/directline.service';
import { DashboardModule } from './modules/dashboard/dashboard.module';
import { AdalService } from './core/services/adal.service';
import { TokenInterceptorService } from './core/services/token-interceptor.service';

@NgModule({
    declarations: [ AppComponent ],
    imports: [
        BrowserModule,
        HttpClientModule,
        DashboardModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        StoreModule.forRoot(reducers, { metaReducers }),
        !environment.production ? StoreDevtoolsModule.instrument() : [],
        EffectsModule.forRoot(effects),
        StoreRouterConnectingModule.forRoot({
            stateKey: 'router', // name of reducer key
        }),
        MaterialModule,
        SharedModule.forRoot(),
        MsAdalAngular6Module.forRoot({
            tenant: environment.azureAd.tenant,
            clientId: environment.azureAd.clientId,
            redirectUri: window.location.origin,
            navigateToLoginRequestUrl: false,
            cacheLocation: 'sessionStorage',
        })
    ],
    providers: [
        AuthenticationGuard,
        MsalService,
        AdalService,
        SignalRService,
        DirectlineService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: TokenInterceptorService,
            multi: true,
        }
    ],
    bootstrap: [ AppComponent ],
})
export class AppModule {
    constructor (signalRService: SignalRService) {
    }
}
