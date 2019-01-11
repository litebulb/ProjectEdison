import { PerfectScrollbarComponent } from 'ngx-perfect-scrollbar';

import { AfterViewChecked, Component, Input, OnChanges, ViewChild } from '@angular/core';

import { Message } from '../../../../reducers/chat/chat.model';

@Component({
    selector: 'app-message-list',
    templateUrl: './message-list.component.html',
    styleUrls: [ './message-list.component.scss' ],

})
export class MessageListComponent implements OnChanges, AfterViewChecked {
    @ViewChild(PerfectScrollbarComponent) componentRef?: PerfectScrollbarComponent;

    @Input() messages: Message[];

    contentChanged: boolean;

    ngOnChanges() {
        this.contentChanged = true;
    }

    ngAfterViewChecked(): void {
        if (this.contentChanged) {
            this._scrollToBottom();
            this.contentChanged = false;
        }
    }


    private _scrollToBottom() {
        this.componentRef.directiveRef.scrollToBottom();
    }
}
