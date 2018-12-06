import { ToastContainerModule } from 'ngx-toastr';

import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { DevicesModule } from '../devices/devices.module';
import { MapModule } from '../map/map.module';
import { MaterialModule } from '../material/material.module';
import { MessagingModule } from '../messaging/messaging.module';
import { SharedModule } from '../shared/shared.module';
import {
    ActivateResponseComponent
} from './components/activate-response/activate-response.component';
import {
    ActiveResponseIconComponent
} from './components/active-response-icon/active-response-icon.component';
import { ActiveResponseComponent } from './components/active-response/active-response.component';
import { ActiveResponsesComponent } from './components/active-responses/active-responses.component';
import {
    DeactivateResponseComponent
} from './components/deactivate-response/deactivate-response.component';
import { EventBarComponent } from './components/event-bar/event-bar.component';
import { EventCardComponent } from './components/event-card/event-card.component';
import { ToastContainerComponent } from './components/toast-container/toast-container.component';
import { UpdateResponseComponent } from './components/update-response/update-response.component';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './pages/dashboard/dashboard.component';

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
        DevicesModule,
        ToastContainerModule
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
        ToastContainerComponent,
    ],
})
export class DashboardModule { }
