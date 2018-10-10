import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SidenavComponent } from './components/sidenav/sidenav.component';
import { MaterialModule } from '../material/material.module';
import { TopnavComponent } from './components/topnav/topnav.component';
import { CircleSpinnerComponent } from './components/circle-spinner/circle-spinner.component';
import { SafePipe } from '../../shared/pipes/safe';
import { RoundProgressModule } from 'angular-svg-round-progressbar';
import { SearchListComponent } from './components/search-list/search-list.component';
import { FormsModule } from '@angular/forms';
import { SearchListItemComponent } from './components/search-list-item/search-list-item.component';
import { ActionListComponent } from './components/action-list/action-list.component';
import { ActionListItemComponent } from './components/action-list-item/action-list-item.component';
import {
  NotificationTemplateComponent
} from './components/action-list-item/templates/notification-template/notification-template.component';
import { RadiusTemplateComponent } from './components/action-list-item/templates/radius-template/radius-template.component';
import { DelayButtonComponent } from './components/delay-button/delay-button.component';
import { TextTemplateComponent } from './components/action-list-item/templates/text-template/text-template.component';
import { NumberDatePipe } from '../../shared/pipes/number-date';

const sharedComponents = [
  SidenavComponent,
  TopnavComponent,
  CircleSpinnerComponent,
  SearchListComponent,
  SearchListItemComponent,
  ActionListComponent,
  ActionListItemComponent,
  DelayButtonComponent,
  NumberDatePipe
];

@NgModule({
  imports: [CommonModule, MaterialModule, FormsModule, RoundProgressModule],
  entryComponents: [
    NotificationTemplateComponent,
    RadiusTemplateComponent,
    TextTemplateComponent,
  ],
  declarations: [
    ...sharedComponents,
    SafePipe,
    NotificationTemplateComponent,
    RadiusTemplateComponent,
    TextTemplateComponent,
  ],
  exports: [...sharedComponents],
})
export class SharedModule {}
