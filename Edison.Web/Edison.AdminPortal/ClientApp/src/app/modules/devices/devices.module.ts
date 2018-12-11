import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { SharedModule } from '../shared/shared.module';
import { DeviceFiltersComponent } from './components/device-filters/device-filters.component';
import { DevicesComponent } from './pages/devices/devices.component';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        FormsModule
    ],
    declarations: [
        DevicesComponent,
        DeviceFiltersComponent
    ],
    exports: [
        DevicesComponent
    ]
})
export class DevicesModule { }
