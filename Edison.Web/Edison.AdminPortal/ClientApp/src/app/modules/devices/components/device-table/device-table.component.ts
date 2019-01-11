import {
    Component, EventEmitter, Input, OnChanges, OnInit, Output, ViewChild
} from '@angular/core';
import { MatSort, MatTableDataSource } from '@angular/material';

import { ExpandedDevice } from '../../models/expanded-device.model';

@Component({
    selector: 'app-device-table',
    templateUrl: './device-table.component.html',
    styleUrls: [ './device-table.component.scss' ]
})
export class DeviceTableComponent implements OnInit, OnChanges {
    @Input() devices: ExpandedDevice[];
    @Input() filter: string;

    @Output() rowClick = new EventEmitter();

    @ViewChild(MatSort) sort: MatSort;

    dataSource: MatTableDataSource<ExpandedDevice>;
    displayedColumns: string[] = [ 'lastAccessTime', 'deviceType', 'fullLocationName', 'currentResponse', 'recentEvents' ];

    constructor () { }

    ngOnInit() {
        this._initDataSource();
        this._filterDataSource();
    }

    ngOnChanges() {
        if (this.dataSource && this.dataSource.data.length <= 0 && this.devices.length > 0) {
            this._initDataSource();
        }
        this._filterDataSource();
    }

    onRowClick(row) {
        this.rowClick.emit(row);
    }

    private _filterDataSource() {
        if (this.filter && this.dataSource) {
            this.dataSource.filter = this.filter;
        }
    }

    private _initDataSource() {
        this.dataSource = new MatTableDataSource(this.devices);
        this.dataSource.sort = this.sort;
        this.dataSource.sortingDataAccessor = (item, property) => {
            switch (property) {
                case 'lastAccessTime': return new Date(item[ property ])
                default: return item[ property ];
            }
        }
        this.dataSource.filterPredicate = (data, filter) => {
            return filter.indexOf(data.deviceType) !== -1 &&
                (filter.indexOf('Online') !== -1 && data.online || !data.online) &&
                (filter.indexOf('Offline') === -1 && !data.online || data.online);
        }
    }

}
