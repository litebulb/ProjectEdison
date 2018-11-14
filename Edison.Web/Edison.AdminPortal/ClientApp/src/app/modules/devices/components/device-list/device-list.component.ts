import { Component, Input, ChangeDetectionStrategy, Output, EventEmitter } from '@angular/core';
import { ExpandedDevice } from '../../models/expanded-device.model';

@Component({
    selector: 'app-device-list',
    templateUrl: './device-list.component.html',
    styleUrls: [ './device-list.component.scss' ],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DeviceListComponent {
    @Input() devices: ExpandedDevice[];
    @Output() onrowclick = new EventEmitter<ExpandedDevice>();

    click(device: ExpandedDevice) {
        this.onrowclick.emit(device);
    }
}
