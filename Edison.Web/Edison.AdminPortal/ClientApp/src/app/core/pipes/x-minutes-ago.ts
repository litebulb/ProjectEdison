import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'xminutesago' })
export class XMinutesAgoPipe implements PipeTransform {
    constructor () { }

    transform(
        value: string
    ): string {
        if (value) {
            const date = new Date(value);
            const currDate = new Date();

            const diffMs = Math.round((currDate.getTime() / 60000) - (date.getTime() / 60000)); // minutes

            return diffMs === 1 ? '1 minute ago' : `${diffMs} minutes ago`;
        }

        return 'Unknown';
    }
}
