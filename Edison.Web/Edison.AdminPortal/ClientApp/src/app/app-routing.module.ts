import { AuthenticationGuard } from 'microsoft-adal-angular6';

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
    {
        path: 'settings',
        loadChildren:
            './modules/settings/settings.module#SettingsModule',
        canLoad: [ AuthenticationGuard ],
        data: {
            title: 'SETTINGS',
        },
    }
]

@NgModule({
    imports: [ RouterModule.forRoot(routes) ],
    exports: [ RouterModule ],
})
export class AppRoutingModule { }
