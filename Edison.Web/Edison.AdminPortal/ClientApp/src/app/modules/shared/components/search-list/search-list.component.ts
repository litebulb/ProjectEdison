import {
    Component, ElementRef, EventEmitter, Input, OnChanges, OnInit, Output, ViewChild
} from '@angular/core';

import { listFadeInOut } from '../../../../core/animations/listFadeInOut';
import { SearchListItem } from '../../../../core/models/searchListItem';

@Component({
    selector: 'app-search-list',
    templateUrl: './search-list.component.html',
    styleUrls: [ './search-list.component.scss' ],
    animations: [ listFadeInOut ],
})
export class SearchListComponent implements OnInit, OnChanges {
    @ViewChild('searchInput') searchInput: ElementRef;
    @Input()
    title: string
    @Input()
    items: SearchListItem[] = null
    @Input()
    activeId: string

    @Output()
    itemSelected = new EventEmitter<SearchListItem>()

    filteredItems: SearchListItem[]
    selectedItem: SearchListItem
    hoveredId: any
    currentFilter = ''

    constructor () { }

    ngOnInit() {
        this.filteredItems = this.items;
        this.updateActiveItem();
        this.searchInput.nativeElement.focus();
    }

    ngOnChanges() {
        this.updateActiveItem()
    }

    updateActiveItem() {
        if (this.activeId) {
            this.selectedItem = this.items.find(item => item.id === this.activeId)
        } else {
            this.selectedItem = null;
        }
    }

    updateSearchFilter(searchFilter: string) {
        this.currentFilter = searchFilter
        if (searchFilter && searchFilter.length > 0 && searchFilter !== ' ') {
            this.filteredItems = this.items.filter(ap => {
                const typeStr: string = ap.name.toLowerCase()
                return typeStr.indexOf(searchFilter.toLowerCase()) !== -1
            })
        } else {
            this.filteredItems = this.items
        }
    }

    clearFilter() {
        if (this.currentFilter.length > 0) {
            this.updateSearchFilter('')
        }
    }

    selectItem(item: SearchListItem) {
        this.itemSelected.emit(item)
        this.selectedItem = item
    }

    isRowActive(item: SearchListItem, hoverId?) {
        if (hoverId) {
            return (
                (hoverId && hoverId === item.id) ||
                (this.selectedItem && this.selectedItem.id === item.id)
            )
        }

        return this.selectedItem && this.selectedItem.id === item.id
    }
}
