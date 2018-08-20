import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss']
})
export class SidenavComponent implements OnInit {
  public navLinks = [
    {
      title: 'Dashboard',
      route: '',
      icon: 'dashboard',
      active: true,
    },
    {
      title: 'Alerts',
      route: 'alerts',
      icon: 'notifications',
    },
    {
      title: 'Action Screen',
      route: 'action-screen',
      icon: 'flash_on',
    },
    {
      title: 'Configuration',
      route: 'configuration',
      icon: 'build',
    },
  ];

  constructor(private router: Router) { }

  ngOnInit() {
  }

  onClick(navLink) {
    this.activateNavLink(navLink);
    this.router.navigate([navLink.route]);
  }

  private activateNavLink(activeNavLink) {
    this.navLinks = this.navLinks.map(nl => ({ ...nl, active: nl.title === activeNavLink.title }));
  }

}
