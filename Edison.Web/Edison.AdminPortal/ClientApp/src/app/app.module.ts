import 'hammerjs';

import { AuthenticationGuard, MsAdalAngular6Module } from 'microsoft-adal-angular6';
import { ToastrModule } from 'ngx-toastr';

import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { EffectsModule } from '@ngrx/effects';
import { StoreRouterConnectingModule } from '@ngrx/router-store';
import { StoreModule } from '@ngrx/store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';

import { environment } from '../environments/environment';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AdalService } from './core/services/adal.service';
import { DirectlineService } from './core/services/directline/directline.service';
import { MsalService } from './core/services/msal.service';
import { SignalRService } from './core/services/signal-r.service';
import { TokenInterceptorService } from './core/services/token-interceptor.service';
import { effects } from './effects';
import { DashboardModule } from './modules/dashboard/dashboard.module';
import { MaterialModule } from './modules/material/material.module';
import { SharedModule } from './modules/shared/shared.module';
import { metaReducers, reducers } from './reducers';

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
        }),
        ToastrModule.forRoot({ positionClass: 'inline', maxOpened: 5 }),
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
