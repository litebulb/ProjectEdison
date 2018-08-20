import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { DeviceListComponent } from './components/device-list/device-list.component';
import { AlertListComponent } from './components/alert-list/alert-list.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    DashboardRoutingModule,
    SharedModule
  ],
  declarations: [DeviceListComponent, AlertListComponent, DashboardComponent]
})
export class DashboardModule { }
