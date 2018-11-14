import {
    Component,
    OnInit,
    Input,
    Output,
    EventEmitter,
    ChangeDetectionStrategy,
} from '@angular/core'
import { SearchListItem } from '../../../../core/models/searchListItem'

@Component({
    selector: 'app-search-list-item',
    templateUrl: './search-list-item.component.html',
    styleUrls: [ './search-list-item.component.scss' ],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SearchListItemComponent implements OnInit {
    @Input()
    item: SearchListItem
    @Input()
    active: boolean

    @Output()
    close = new EventEmitter<Event>()
    @Output()
    click = new EventEmitter<SearchListItem>()
    @Output()
    mouseenter = new EventEmitter()
    @Output()
    mouseleave = new EventEmitter()

    constructor () { }

    ngOnInit() { }

    getIconClass() {
        return `${this.item.icon.toLowerCase()} ${this.active ? 'active' : ''}`
    }

    getRowClass() {
        return `${this.item.color.toLowerCase()} ${this.active ? 'active' : ''}`
    }

    onClick(event: Event) {
        event.stopPropagation()
        this.click.emit(this.item)
    }

    onClose(event: Event) {
        event.stopPropagation()
        this.close.emit(event)
    }
}
