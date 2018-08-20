import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'configuration',
    loadChildren:
      './modules/configuration/configuration.module#ConfigurationModule',
  },
  {
    path: 'alerts',
    loadChildren: './modules/alerts/alerts.module#AlertsModule',
  },
  {
    path: 'action-screen',
    loadChildren:
      './modules/action-screen/action-screen.module#ActionScreenModule',
  },
  {
    path: '',
    redirectTo: './modules/dashboard/dashboard.module#DashboardModule',
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
