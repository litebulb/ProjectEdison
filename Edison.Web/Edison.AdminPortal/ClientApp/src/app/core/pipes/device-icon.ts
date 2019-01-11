import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'deviceicon' })
export class DeviceIconPipe implements PipeTransform {
    constructor () { }

    transform(
        value: string
    ): string {
        if (value) {
            const loweredValue = value.toLowerCase();
            if (loweredValue.toLowerCase().includes('button')) { return 'button' }
            if (loweredValue.toLowerCase().includes('sound')) { return 'sound' }
            if (loweredValue.toLowerCase().includes('mobile')) { return 'chat' }
            if (loweredValue.toLowerCase().includes('bulb')) { return 'light' }
        }

        return '';
    }
}
