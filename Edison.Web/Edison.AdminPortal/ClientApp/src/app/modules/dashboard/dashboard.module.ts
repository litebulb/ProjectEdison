import { NgModule } from '@angular/core'
import { CommonModule } from '@angular/common'
import { FormsModule } from '@angular/forms'

import { DashboardRoutingModule } from './dashboard-routing.module'
import { DashboardComponent } from './pages/dashboard/dashboard.component'
import { SharedModule } from '../shared/shared.module'
import { MaterialModule } from '../material/material.module'
import { MapModule } from '../map/map.module'
import { EventBarComponent } from './components/event-bar/event-bar.component'
import { EventCardComponent } from './components/event-card/event-card.component'
import { ActivateResponseComponent } from './components/activate-response/activate-response.component'
import { ActiveResponsesComponent } from './components/active-responses/active-responses.component'
import { ActiveResponseIconComponent } from './components/active-response-icon/active-response-icon.component'
import { DeactivateResponseComponent } from './components/deactivate-response/deactivate-response.component'
import { ActiveResponseComponent } from './components/active-response/active-response.component'
import { UpdateResponseComponent } from './components/update-response/update-response.component'
import { MessagingModule } from '../messaging/messaging.module';
import { DevicesModule } from '../devices/devices.module';

// import { SmartMapModule } from 'three-psa-ui-lib';

@NgModule({
    imports: [
        CommonModule,
        DashboardRoutingModule,
        SharedModule,
        MaterialModule,
        MapModule,
        FormsModule,
        MessagingModule,
        DevicesModule
        // SmartMapModule
    ],
    declarations: [
        DashboardComponent,
        EventBarComponent,
        EventCardComponent,
        ActivateResponseComponent,
        ActiveResponsesComponent,
        ActiveResponseIconComponent,
        DeactivateResponseComponent,
        ActiveResponseComponent,
        UpdateResponseComponent,
    ],
})
export class DashboardModule { }
