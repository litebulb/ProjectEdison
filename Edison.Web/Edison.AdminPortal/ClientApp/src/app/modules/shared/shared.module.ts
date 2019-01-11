import { RoundProgressModule } from 'angular-svg-round-progressbar';
import {
    PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface, PerfectScrollbarModule
} from 'ngx-perfect-scrollbar';

import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { NumberDatePipe, SafePipe } from '../../core/pipes';
import { DeviceIconPipe } from '../../core/pipes/device-icon';
import { XMinutesAgoPipe } from '../../core/pipes/x-minutes-ago';
import { MaterialModule } from '../material/material.module';
import {
    ActionListItemStateComponent
} from './components/action-list-item-state/action-list-item-state.component';
import { ActionListItemComponent } from './components/action-list-item/action-list-item.component';
import {
    NotificationTemplateComponent
} from './components/action-list-item/templates/notification-template/notification-template.component';
import {
    RadiusTemplateComponent
} from './components/action-list-item/templates/radius-template/radius-template.component';
import {
    TextTemplateComponent
} from './components/action-list-item/templates/text-template/text-template.component';
import { ActionListComponent } from './components/action-list/action-list.component';
import { CircleSpinnerComponent } from './components/circle-spinner/circle-spinner.component';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { DelayButtonComponent } from './components/delay-button/delay-button.component';
import { IconComponent } from './components/icon/icon.component';
import { SearchListItemComponent } from './components/search-list-item/search-list-item.component';
import { SearchListComponent } from './components/search-list/search-list.component';
import { SidenavComponent } from './components/sidenav/sidenav.component';
import { TopnavComponent } from './components/topnav/topnav.component';

const sharedComponents = [
    SidenavComponent,
    TopnavComponent,
    CircleSpinnerComponent,
    SearchListComponent,
    SearchListItemComponent,
    ActionListComponent,
    ActionListItemComponent,
    DelayButtonComponent,
    NumberDatePipe,
    ConfirmDialogComponent,
    ActionListItemStateComponent,
    IconComponent,
]

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
    suppressScrollX: true
};

@NgModule({
    imports: [
        CommonModule,
        MaterialModule,
        FormsModule,
        RoundProgressModule,
        PerfectScrollbarModule
    ],
    entryComponents: [
        NotificationTemplateComponent,
        RadiusTemplateComponent,
        TextTemplateComponent,
        ConfirmDialogComponent,
    ],
    declarations: [
        ...sharedComponents,
        SafePipe,
        XMinutesAgoPipe,
        DeviceIconPipe,
        NotificationTemplateComponent,
        RadiusTemplateComponent,
        TextTemplateComponent,
    ],
    exports: [
        ...sharedComponents,
        PerfectScrollbarModule,
        MaterialModule,
        SafePipe,
        XMinutesAgoPipe,
        DeviceIconPipe,
    ],
    providers: [
        {
            provide: PERFECT_SCROLLBAR_CONFIG,
            useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
        }
    ]
})
export class SharedModule {
    static forRoot(): ModuleWithProviders {
        return {
            ngModule: SharedModule,
            providers: [
                {
                    provide: PERFECT_SCROLLBAR_CONFIG,
                    useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
                }
            ]
        };
    }
}
