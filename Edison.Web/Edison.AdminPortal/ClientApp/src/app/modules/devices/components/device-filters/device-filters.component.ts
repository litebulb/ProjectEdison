import { Component, Input } from '@angular/core';

import { FilterGroupModel } from '../../models/filter-group.model';

@Component({
    selector: 'app-device-filters',
    templateUrl: './device-filters.component.html',
    styleUrls: [ './device-filters.component.scss' ]
})
export class DeviceFiltersComponent {
    @Input() filterGroups: FilterGroupModel[];
}
