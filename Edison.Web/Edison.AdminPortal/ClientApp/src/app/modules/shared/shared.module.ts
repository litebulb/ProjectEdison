import { NgModule, ModuleWithProviders } from '@angular/core'
import { CommonModule } from '@angular/common'
import { SidenavComponent } from './components/sidenav/sidenav.component'
import { MaterialModule } from '../material/material.module'
import { TopnavComponent } from './components/topnav/topnav.component'
import { CircleSpinnerComponent } from './components/circle-spinner/circle-spinner.component'
import { SafePipe } from '../../core/pipes/safe'
import { RoundProgressModule } from 'angular-svg-round-progressbar'
import { SearchListComponent } from './components/search-list/search-list.component'
import { FormsModule } from '@angular/forms'
import { SearchListItemComponent } from './components/search-list-item/search-list-item.component'
import { ActionListComponent } from './components/action-list/action-list.component'
import { ActionListItemComponent } from './components/action-list-item/action-list-item.component'
import { NotificationTemplateComponent } from './components/action-list-item/templates/notification-template/notification-template.component'
import { RadiusTemplateComponent } from './components/action-list-item/templates/radius-template/radius-template.component'
import { DelayButtonComponent } from './components/delay-button/delay-button.component'
import { TextTemplateComponent } from './components/action-list-item/templates/text-template/text-template.component'
import { NumberDatePipe } from '../../core/pipes/number-date';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component'
import {
    PerfectScrollbarConfigInterface,
    PerfectScrollbarModule,
    PERFECT_SCROLLBAR_CONFIG
} from 'ngx-perfect-scrollbar';

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
    exports: [ ...sharedComponents, PerfectScrollbarModule, MaterialModule ],
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
