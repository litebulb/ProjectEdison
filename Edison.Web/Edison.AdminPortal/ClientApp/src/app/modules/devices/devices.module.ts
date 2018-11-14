import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DevicesComponent } from './pages/devices/devices.component';
import { DeviceListComponent } from './components/device-list/device-list.component';
import { DeviceRowComponent } from './components/device-row/device-row.component';
import { DeviceFiltersComponent } from './components/device-filters/device-filters.component';
import { SharedModule } from '../shared/shared.module';
import { FormsModule } from '@angular/forms';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        FormsModule
    ],
    declarations: [
        DevicesComponent,
        DeviceListComponent,
        DeviceRowComponent,
        DeviceFiltersComponent
    ],
    exports: [
        DevicesComponent
    ]
})
export class DevicesModule { }
