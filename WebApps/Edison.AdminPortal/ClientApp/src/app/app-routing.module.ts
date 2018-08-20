import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'configuration',
    loadChildren: './modules/configuration/configuration.module#ConfigurationModule',
  },
  {
    path: 'alerts',
    loadChildren: './modules/alerts/alerts.module#AlertsModule',
  },
  {
    path: 'action-screen',
    loadChildren: './modules/action-screen/action-screen.module#ActionScreenModule',
  },
  {
    path: 'dashboard',
    loadChildren: './modules/dashboard/dashboard.module#DashboardModule',
  },
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full',
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
