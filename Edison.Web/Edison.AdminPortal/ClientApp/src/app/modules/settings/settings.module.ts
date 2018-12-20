import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SharedModule } from '../shared/shared.module';
import {
    ActionPlanListItemComponent
} from './components/action-plan-list-item/action-plan-list-item.component';
import { ActionPlanListComponent } from './components/action-plan-list/action-plan-list.component';
import { HeaderComponent } from './components/header/header.component';
import { ResponsesComponent } from './pages/responses/responses.component';
import { SettingsComponent } from './pages/settings/settings.component';
import { SettingsRoutingModule } from './settings-routing.module';

@NgModule({
    imports: [
        CommonModule,
        SettingsRoutingModule,
        SharedModule,
        FormsModule,
        ReactiveFormsModule
    ],
    declarations: [
        SettingsComponent,
        ResponsesComponent,
        HeaderComponent,
        ActionPlanListComponent,
        ActionPlanListItemComponent,
    ]
})
export class SettingsModule { }
