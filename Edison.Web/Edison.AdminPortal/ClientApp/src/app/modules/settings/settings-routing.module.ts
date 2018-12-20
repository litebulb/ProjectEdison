import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CanDeactivateGuard } from '../../core/services/can-deactivate-guard.service';
import { ResponsesComponent } from './pages/responses/responses.component';
import { SettingsComponent } from './pages/settings/settings.component';

const routes: Routes = [
    {
        path: 'settings',
        pathMatch: 'full',
        component: SettingsComponent,
        data: {
            title: 'SETTINGS',
            sidebar: false,

        },
        children: [
            {
                path: '',
                component: ResponsesComponent,
                canDeactivate: [ CanDeactivateGuard ],
            }
        ]
    },
]

@NgModule({
    imports: [ RouterModule.forChild(routes) ],
    exports: [ RouterModule ],
    providers: [ CanDeactivateGuard ]
})
export class SettingsRoutingModule { }
