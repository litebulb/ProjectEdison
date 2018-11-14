import { Component, Input, ChangeDetectionStrategy, Output, EventEmitter } from '@angular/core';
import { ExpandedDevice } from '../../models/expanded-device.model';
import { ResponseState } from '../../../../reducers/response/response.model';

@Component({
    selector: 'app-device-row',
    templateUrl: './device-row.component.html',
    styleUrls: [ './device-row.component.scss' ],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DeviceRowComponent {
    @Input() device: ExpandedDevice;
    @Output() onclick = new EventEmitter<ExpandedDevice>();

    click() {
        this.onclick.emit(this.device);
    }

    getResponseIcon() {
        const { response } = this.device;
        if (!response || response.responseState === ResponseState.Inactive) { return; }

        return `${response.icon.toLowerCase()}-static ${response.color.toLowerCase()}`
    }

    getDeviceIcon() {
        if (this.device.deviceType.toLowerCase().includes('button')) { return 'button' }
        if (this.device.deviceType.toLowerCase().includes('sound')) { return 'sound' }
        if (this.device.deviceType.toLowerCase().includes('mobile')) { return 'chat' }
        if (this.device.deviceType.toLowerCase().includes('light')) { return 'light' }
    }

    getLastAccessTime() {
        const { lastAccessTime } = this.device;
        if (lastAccessTime) {
            const date = new Date(lastAccessTime);
            const currDate = new Date();

            const diffMs = currDate.getTime() - date.getTime(); // milliseconds
            const diffMins = Math.round(((diffMs % 86400000) % 3600000) / 60000);

            return diffMins === 1 ? '1 minute ago' : `${diffMins} minutes ago`;
        }

        return '';
    }
}
