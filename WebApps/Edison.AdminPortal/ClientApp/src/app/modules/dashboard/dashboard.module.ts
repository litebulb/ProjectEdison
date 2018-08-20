import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { DefaultComponent } from './components/default/default.component';
import { DeviceListComponent } from './components/device-list/device-list.component';
import { AlertListComponent } from './components/alert-list/alert-list.component';

@NgModule({
  imports: [
    CommonModule,
    DashboardRoutingModule
  ],
  declarations: [DefaultComponent, DeviceListComponent, AlertListComponent]
})
export class DashboardModule { }
