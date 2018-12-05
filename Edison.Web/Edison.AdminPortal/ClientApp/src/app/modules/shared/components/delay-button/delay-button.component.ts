import { interval, Subscription } from 'rxjs';
import { startWith, take } from 'rxjs/operators';

import {
    ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Input, Output
} from '@angular/core';

@Component({
    selector: 'app-delay-button',
    templateUrl: './delay-button.component.html',
    styleUrls: [ './delay-button.component.scss' ],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DelayButtonComponent {
    @Input()
    buttonText: string
    @Input()
    subheaderText: string
    @Input()
    disabled = false
    @Input()
    urgency: string
    @Output()
    clickCompleted = new EventEmitter()

    constructor (private cdr: ChangeDetectorRef) { }

    private intervalSub$: Subscription

    currentProgress = 0
    maxProgress = 100
    progressMaxed = false
    progressInterval: any
    hover = false
    active = false
    activated = false

    activateInterval = () => {
        if (this.disabled || (this.intervalSub$ && !this.intervalSub$.closed)) {
            return
        }

        this.active = true

        this.intervalSub$ = interval(500)
            .pipe(
                startWith(1),
                take(9)
            )
            .subscribe(x => {
                if (this.currentProgress >= this.maxProgress) {
                    this.intervalSub$.unsubscribe();
                    this.progressMaxed = true;
                    this.clickCompleted.emit();
                } else {
                    this.currentProgress += this.maxProgress / 8;
                }
                this.cdr.markForCheck();
            })
    }

    deactivateInterval = () => {
        if (this.disabled) {
            return
        }

        this.intervalSub$ && this.intervalSub$.unsubscribe()

        this.active = false
        this.currentProgress = 0
        this.cdr.markForCheck();
    }

    getCircleClass() {
        return `${this.active ? 'active' : ''} ${this.urgency &&
            this.urgency.toLowerCase()}`
    }
}
