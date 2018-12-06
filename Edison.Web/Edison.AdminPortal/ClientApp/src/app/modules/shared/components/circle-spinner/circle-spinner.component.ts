import {
    ChangeDetectionStrategy, Component, ElementRef, EventEmitter, Input, OnChanges, OnInit, Output,
    ViewChild
} from '@angular/core';

@Component({
    selector: 'app-circle-spinner',
    templateUrl: './circle-spinner.component.html',
    styleUrls: [ './circle-spinner.component.scss' ],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CircleSpinnerComponent implements OnInit, OnChanges {
    @ViewChild('spinner') elementRef: ElementRef;

    @Input() size: number;
    @Input() depth: number;
    @Input() spinnerColor: string;
    @Input() circleColor: string;
    @Input() fillColor: string;
    @Input() fontColor: string;
    @Input() animate = true;
    @Input() tooltip: boolean;
    @Output() click = new EventEmitter<void>();

    circleSize: string;
    subCircleHeight: string;
    subCircleWidth: string;
    subCircleGradient1: string;
    subCircleGradient2: string;
    innerCircleSize: string;
    innerCircleOffset: string;
    contentCircleSize: string;
    contentCircleOffset: string;
    contentText: string;
    opacity = 0;

    ngOnInit() {
        if (!this.animate) {
            this.opacity = 1;
        }
        this.setup(true);
    }

    ngOnChanges() {
        this.setup();
    }

    setup(showEntrance = false) {
        this.circleSize = `${this.size}px`;
        this.subCircleHeight = `${this.size / 2}px`;
        this.subCircleWidth = `${this.size}px`;
        this.innerCircleSize = `${this.size - this.depth}px`;
        this.innerCircleOffset = `${this.depth / 2}px`;
        this.contentCircleSize = `${this.size - (this.depth * 2)}px`;
        this.contentCircleOffset = `${(this.depth * 2) / 2}px`;

        if (this.animate) {
            this.subCircleGradient1 = `linear-gradient(to right, rgba(${this.spinnerColor}, .4) 0%, rgba(${this.spinnerColor}, .7) 100%)`;
            this.subCircleGradient2 = `linear-gradient(to right, rgba(${this.spinnerColor}, 1) 0%, rgba(${this.spinnerColor}, .7) 100%)`;
        } else {
            this.subCircleGradient1 = `rgba(${this.spinnerColor}, 1)`;
            this.subCircleGradient2 = `rgba(${this.spinnerColor}, 1)`;
        }

        if (showEntrance) {
            this.opacity = 1;
        }
    }

    onClick() {
        this.click.emit();
    }

    getSpinnerElement(text: string, tooltip?: string, clickable?: boolean) {
        const node = this.elementRef.nativeElement.cloneNode(true);

        node.getElementsByClassName('content-circle')[ 0 ].innerHTML = text;
        if (tooltip) {
            const element = node.getElementsByClassName('tooltip')[ 0 ];
            element.innerHTML = tooltip;

            const elementContainer = node.getElementsByClassName('tooltip-container')[ 0 ];
            elementContainer.style.visibility = 'visible';
        }

        if (clickable) {
            node.style.cursor = 'pointer';
        }

        return node;
    }
}
