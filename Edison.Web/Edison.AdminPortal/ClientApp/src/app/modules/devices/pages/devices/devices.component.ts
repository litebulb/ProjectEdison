import { Component, OnInit, OnDestroy } from '@angular/core';
import { FilterGroupModel } from '../../models/filter-group.model';
import { Store, select } from '@ngrx/store';
import { AppState } from '../../../../reducers';
import { Subscription, combineLatest } from 'rxjs';
import { devicesFilteredSelector } from '../../../../reducers/device/device.selectors';
import { responsesSelector } from '../../../../reducers/response/response.selectors';
import { eventsSelector } from '../../../../reducers/event/event.selectors';
import { ExpandedDevice } from '../../models/expanded-device.model';
import { Event } from '../../../../reducers/event/event.model';
import { Response } from '../../../../reducers/response/response.model';
import { TestDevice } from '../../../../reducers/device/device.actions';
import { SetPageData } from '../../../../reducers/app/app.actions';

@Component({
    selector: 'app-devices',
    templateUrl: './devices.component.html',
    styleUrls: [ './devices.component.scss' ]
})
export class DevicesComponent implements OnInit, OnDestroy {
    filters: FilterGroupModel[];
    devices: ExpandedDevice[];
    filteredDevices: ExpandedDevice[];
    combinedStream$: Subscription;

    private showButtons: boolean = true;
    private showSoundSensors: boolean = true;
    private showOnline: boolean = true;
    private showOffline: boolean = true;

    constructor (private store: Store<AppState>) { }

    ngOnInit() {
        this.setupSubscriptions();
        this.setupFilters();

        this.store.dispatch(new SetPageData({ title: 'DEVICES', sidebar: false }))
    }

    ngOnDestroy() {
        this.combinedStream$.unsubscribe();
    }

    onRowClick(device: ExpandedDevice) {
        this.store.dispatch(new TestDevice(device));
    }

    private setupSubscriptions() {
        const devicesObs = this.store.pipe(select(devicesFilteredSelector));
        const responsesObs = this.store.pipe(select(responsesSelector));
        const eventsObs = this.store.pipe(select(eventsSelector));

        this.combinedStream$ = combineLatest(devicesObs, responsesObs, eventsObs).subscribe(([ devices, responses, events ]) => {
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
                .sort((a, b) => b.startDate - a.startDate)
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
            return (device.deviceType.toLowerCase() !== 'buttonsensor' || this.showButtons) &&
                (device.deviceType.toLowerCase() !== 'soundsensor' || this.showSoundSensors) &&
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
