import { NgModule } from '@angular/core'
import { Routes, RouterModule } from '@angular/router'
import { AuthenticationGuard } from 'microsoft-adal-angular6';

const routes: Routes = [
    {
        path: 'configuration',
        loadChildren:
            './modules/configuration/configuration.module#ConfigurationModule',
        canLoad: [ AuthenticationGuard ],
        data: {
            title: 'CONFIGURATION',
        },
    }
]

@NgModule({
    imports: [ RouterModule.forRoot(routes) ],
    exports: [ RouterModule ],
})
export class AppRoutingModule { }
