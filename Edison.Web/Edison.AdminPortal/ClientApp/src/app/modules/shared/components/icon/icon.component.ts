import { Component, Input } from '@angular/core';

import { IconSize, IconType } from './icon.models';

@Component({
    selector: 'app-icon',
    templateUrl: './icon.component.html',
    styleUrls: [ './icon.component.scss' ]
})
export class IconComponent {
    @Input() icon: IconType;
    @Input() size: IconSize = 'medium';
    @Input() color: string = '';
    @Input() round: boolean = false;
    @Input() wide: boolean = false;
    @Input() static: boolean = false;
    @Input() staticSize: boolean = false;
    @Input() hover: boolean = false;
    @Input() active: boolean = false;
    @Input() style: any;

    getIconClass() {
        this.validateProps();

        let iconClass = `${this.icon.toLowerCase()} ${this.size.toLowerCase()} ${this.color.toLowerCase()}`;

        if (this.round) { iconClass += ' round' };
        if (this.wide) { iconClass += ' wide' };
        if (this.static) { iconClass += ' static' };
        if (this.hover) { iconClass += ' hover' };
        if (this.active) { iconClass += ' active' };
        if (this.staticSize) { iconClass += ' static-size' };

        return iconClass;
    }

    validateProps() {
        if (!this.icon) {
            throw new Error('Icon Class required for App Icon Component');
        }
    }
}
