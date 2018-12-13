import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'numberdate' })
export class NumberDatePipe implements PipeTransform {
    constructor () { }

    transform(
        value: number
    ): Date {
        return new Date(value * 1000);
    }
}
