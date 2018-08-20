import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.scss']
})
export class DeviceListComponent implements OnInit {
  public devices = [
    {
      title: 'device 1',
    },
    {
      title: 'device 2',
    },
    {
      title: 'device 3',
    },
    {
      title: 'device 4',
    }
  ];

  constructor() { }

  ngOnInit() {
  }

}
