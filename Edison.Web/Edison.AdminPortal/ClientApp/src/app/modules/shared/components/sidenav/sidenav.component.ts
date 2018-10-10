import { Component, ChangeDetectionStrategy } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidenavComponent {
  public navLinks = [];

  constructor(private router: Router) {
    this.navLinks = [
      {
        title: 'Dashboard',
        route: '',
        icon: 'app-icon shield',
        active: true,
        onClick: this.activateNavLink,
      },
      {
        title: 'Alerts',
        route: 'alerts',
        icon: 'app-icon location-circle',
        onClick: this.activateNavLink,
      },
      {
        title: 'Action Screen',
        route: 'action-screen',
        icon: 'app-icon history',
        onClick: this.activateNavLink,
      },
      {
        title: 'Configuration',
        route: 'configuration',
        icon: 'app-icon gear',
        onClick: this.activateNavLink,
      },
    ];
  }

  private activateNavLink = activeNavLink => {
    this.navLinks = this.navLinks.map(nl => ({
      ...nl,
      active: nl.title === activeNavLink.title,
    }));
    this.router.navigate([activeNavLink.route]);
  }
}
