import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { MaterialModule } from '../material/material.module';
import { DateRangeComponent } from './components/date-range/date-range.component';
import { HistoryComponent } from './pages/history/history.component';

@NgModule({
    imports: [
        CommonModule,
        MaterialModule,
        FormsModule
    ],
    declarations: [ HistoryComponent, DateRangeComponent ]
})
export class HistoryModule { }
