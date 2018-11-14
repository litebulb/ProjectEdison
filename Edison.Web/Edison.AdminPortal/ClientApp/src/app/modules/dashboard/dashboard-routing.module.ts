import { NgModule } from '@angular/core'
import { Routes, RouterModule } from '@angular/router'
import { AuthenticationGuard } from 'microsoft-adal-angular6';

import { DashboardComponent } from './pages/dashboard/dashboard.component'
import { EventBarComponent } from './components/event-bar/event-bar.component'
import { RecentlyActiveComponent } from '../messaging/components/recently-active/recently-active.component'
import { DevicesComponent } from '../devices/pages/devices/devices.component';

const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard',
        canActivate: [ AuthenticationGuard ]
    },
    {
        path: 'dashboard',
        component: DashboardComponent,
        children: [
            {
                path: '',
                component: EventBarComponent,
                data: {
                    title: 'RIGHT NOW',
                    sidebar: true,
                },
            },
            {
                path: 'messaging',
                component: RecentlyActiveComponent,
                data: {
                    title: 'MESSAGING',
                    sidebar: true,
                },
            },
            {
                path: 'devices',
                component: DevicesComponent,
                data: {
                    title: 'DEVICES',
                    sidebar: false,
                }
            }
        ],
    }
]

@NgModule({
    imports: [ RouterModule.forChild(routes) ],
    exports: [ RouterModule ],
})
export class DashboardRoutingModule { }
