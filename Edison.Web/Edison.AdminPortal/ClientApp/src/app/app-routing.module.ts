import { AuthenticationGuard } from 'microsoft-adal-angular6';

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
    {
        path: 'configuration',
        loadChildren:
            './modules/configuration/configuration.module#ConfigurationModule',
        canLoad: [ AuthenticationGuard ],
        data: {
            title: 'CONFIGURATION',
        },
    },
    {
        path: 'history',
        loadChildren:
            './modules/history/history.module#HistoryModule',
        canLoad: [ AuthenticationGuard ],
        data: {
            title: 'HISTORY',
        },
    }
]

@NgModule({
    imports: [ RouterModule.forRoot(routes) ],
    exports: [ RouterModule ],
})
export class AppRoutingModule { }
