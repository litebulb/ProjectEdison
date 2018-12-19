import {
    AfterViewInit, Component, ElementRef, EventEmitter, Output, ViewChild
} from '@angular/core';

@Component({
    selector: 'app-date-range',
    templateUrl: './date-range.component.html',
    styleUrls: [ './date-range.component.scss' ]
})
export class DateRangeComponent implements AfterViewInit {
    startDate: Date;
    endDate: Date;

    @ViewChild('startDateInput') startDateInput: ElementRef;

    @Output() validRangeSelected = new EventEmitter<boolean>();
    @Output() rangeSelected = new EventEmitter<{ startDate: Date, endDate: Date }>();

    constructor () { }

    ngAfterViewInit() {
        // bump to end of the queue, causes expression changed after checked, seems to be an angular material issue
        setTimeout(() => { this.startDateInput.nativeElement.focus(); })
    }

    validateStartDate() {
        if (!this.endDate) { this._emitInvalidResult(); return; }

        if (this.startDate.getTime() > this.endDate.getTime()) { this._emitInvalidResult(); return; }

        this._emitValidResult();
    }

    validateEndDate() {
        if (!this.startDate) { this._emitInvalidResult(); return; }

        if (this.endDate.getTime() < this.startDate.getTime()) { this._emitInvalidResult(); return; }

        this._emitValidResult();
    }

    private _emitValidResult() {
        this.validRangeSelected.emit(true);
        this.rangeSelected.emit({ startDate: this.startDate, endDate: this.endDate });
    }

    private _emitInvalidResult() {
        this.validRangeSelected.emit(false);
    }

}
