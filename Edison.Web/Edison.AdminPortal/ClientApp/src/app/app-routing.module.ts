import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuardService } from './shared/services/auth-guard.service';

const routes: Routes = [
  {
    path: 'configuration',
    loadChildren: './modules/configuration/configuration.module#ConfigurationModule',
    canLoad: [AuthGuardService]
  },
  {
    path: 'alerts',
    loadChildren: './modules/alerts/alerts.module#AlertsModule',
    canLoad: [AuthGuardService]
  },
  {
    path: 'action-screen',
    loadChildren: './modules/action-screen/action-screen.module#ActionScreenModule',
    canLoad: [AuthGuardService]
  },
  {
    path: '',
    loadChildren: './modules/dashboard/dashboard.module#DashboardModule',
    canLoad: [AuthGuardService],
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
