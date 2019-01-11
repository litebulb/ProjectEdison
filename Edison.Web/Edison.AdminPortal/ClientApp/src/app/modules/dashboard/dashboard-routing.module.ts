import { AuthenticationGuard } from 'microsoft-adal-angular6';

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DevicesComponent } from '../devices/pages/devices/devices.component';
import { HistoryComponent } from '../history/pages/history/history.component';
import { MessagingComponent } from '../messaging/pages/messaging/messaging.component';
import { EventBarComponent } from './components/event-bar/event-bar.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';

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
                component: MessagingComponent,
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
            },
            {
                path: 'history',
                component: HistoryComponent,
                data: {
                    title: 'HISTORY',
                    sidebar: true,
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
