import { combineLatest, Subscription } from 'rxjs';

import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { select, Store } from '@ngrx/store';

import { DeviceType } from '../../../../core/models/deviceType';
import { AppState } from '../../../../reducers';
import { SetPageData } from '../../../../reducers/app/app.actions';
import { TestDevice } from '../../../../reducers/device/device.actions';
import { devicesSelector } from '../../../../reducers/device/device.selectors';
import { Event } from '../../../../reducers/event/event.model';
import { eventsSelector } from '../../../../reducers/event/event.selectors';
import { Response } from '../../../../reducers/response/response.model';
import { responsesSelector } from '../../../../reducers/response/response.selectors';
import { ExpandedDevice } from '../../models/expanded-device.model';
import { FilterGroupModel } from '../../models/filter-group.model';

@Component({
    selector: 'app-devices',
    templateUrl: './devices.component.html',
    styleUrls: [ './devices.component.scss' ]
})
export class DevicesComponent implements OnInit, OnDestroy, AfterViewInit {
    @ViewChild('container') container: ElementRef;

    filters: FilterGroupModel[];
    devices: ExpandedDevice[];
    filteredDevices: ExpandedDevice[];
    combinedStream$: Subscription;

    private showButtons: boolean = true;
    private showSoundSensors: boolean = true;
    private showSmartBulbs: boolean = true;
    private showOnline: boolean = true;
    private showOffline: boolean = true;

    constructor (private store: Store<AppState>) { }

    ngOnInit() {
        this.setupSubscriptions();
        this.setupFilters();

        this.store.dispatch(new SetPageData({ title: 'DEVICES', sidebar: false }))
    }

    ngAfterViewInit() {
        this.container.nativeElement.focus();
    }

    ngOnDestroy() {
        this.combinedStream$.unsubscribe();
    }

    onRowClick(device: ExpandedDevice) {
        if (device.deviceType === DeviceType.SmartBulb) {
            this.store.dispatch(new TestDevice(device));
        }
    }

    private setupSubscriptions() {
        const devicesObs = this.store.pipe(select(devicesSelector));
        const responsesObs = this.store.pipe(select(responsesSelector));
        const eventsObs = this.store.pipe(select(eventsSelector));

        this.combinedStream$ = combineLatest(devicesObs, responsesObs, eventsObs)
            .subscribe(([ devices, responses, events ]) => {
                this.devices = devices;
                this.expandDevices(responses, events);
                this.filterDevices();
            })
    }

    private expandDevices(responses: Response[], events: Event[]) {
        this.devices = this.devices.map(device => {
            const result = { ...device };

            const recentEvent = events
                .filter(event => event.device.deviceId === device.deviceId)
                .sort((a, b) => b.startDate.getTime() - a.startDate.getTime())
                .slice(0, 1)[ 0 ];

            if (recentEvent) {
                result.event = recentEvent;
                const response = responses.find(
                    response =>
                        response.primaryEventClusterId === recentEvent.eventClusterId ||
                        response.eventClusterIds.some(eci => eci === recentEvent.eventClusterId)
                )

                if (response) {
                    result.response = response;
                }
            }


            return result;
        });
    }

    private filterDevices() {
        this.filteredDevices = this.devices.filter(device => {
            return (device.deviceType !== DeviceType.ButtonSensor || this.showButtons) &&
                (device.deviceType !== DeviceType.SoundSensor || this.showSoundSensors) &&
                (device.deviceType !== DeviceType.SmartBulb || this.showSmartBulbs) &&
                (this.showOnline && device.online || this.showOffline && !device.online)
        });
    }

    private setupFilters() {
        const typeFilters: FilterGroupModel = {
            title: 'TYPE',
            filters: [
                {
                    title: 'Buttons',
                    onClick: (checked: boolean) => { this.showButtons = checked; this.filterDevices() },
                    iconClasses: 'app-icon medium-small grey button',
                    checked: true,
                },
                {
                    title: 'Sound Sensors',
                    onClick: (checked: boolean) => { this.showSoundSensors = checked; this.filterDevices() },
                    iconClasses: 'app-icon medium-small grey sound',
                    checked: true,
                },
                {
                    title: 'SmartBulbs',
                    onClick: (checked: boolean) => { this.showSmartBulbs = checked; this.filterDevices() },
                    iconClasses: 'app-icon medium-small grey light',
                    checked: true,
                }
            ],
        };

        const connFilters: FilterGroupModel = {
            title: 'CONNECTION',
            filters: [
                {
                    title: 'Online',
                    onClick: (checked: boolean) => { this.showOnline = checked; this.filterDevices() },
                    checked: true,
                    iconClasses: 'app-icon dot active'
                },
                {
                    title: 'Offline',
                    onClick: (checked: boolean) => { this.showOffline = checked; this.filterDevices() },
                    checked: true,
                    iconClasses: 'app-icon dot'
                }
            ]
        };

        this.filters = [ typeFilters, connFilters, ];
    }

}
