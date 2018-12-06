import { RoundProgressModule } from 'angular-svg-round-progressbar';
import {
    PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface, PerfectScrollbarModule
} from 'ngx-perfect-scrollbar';

import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { NumberDatePipe } from '../../core/pipes/number-date';
import { SafePipe } from '../../core/pipes/safe';
import { MaterialModule } from '../material/material.module';
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
        NotificationTemplateComponent,
        RadiusTemplateComponent,
        TextTemplateComponent,
    ],
    exports: [ ...sharedComponents, PerfectScrollbarModule, MaterialModule, SafePipe ],
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
