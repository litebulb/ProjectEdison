import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-alert-list',
  templateUrl: './alert-list.component.html',
  styleUrls: ['./alert-list.component.scss']
})
export class AlertListComponent implements OnInit {
  public alerts = [
    {
      title: 'alert 1',
    },
    {
      title: 'alert 2',
    },
    {
      title: 'alert 3',
    },
    {
      title: 'alert 4',
    }
  ];

  constructor() { }

  ngOnInit() {
  }

}
