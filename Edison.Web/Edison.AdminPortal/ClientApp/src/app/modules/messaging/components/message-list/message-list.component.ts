import { Component, ViewChild, Input, OnChanges, AfterViewInit, AfterViewChecked } from '@angular/core'
import { Message } from '../../../../reducers/chat/chat.model';
import { PerfectScrollbarComponent } from 'ngx-perfect-scrollbar';

@Component({
    selector: 'app-message-list',
    templateUrl: './message-list.component.html',
    styleUrls: [ './message-list.component.scss' ],

})
export class MessageListComponent implements OnChanges, AfterViewChecked {
    @ViewChild(PerfectScrollbarComponent) componentRef?: PerfectScrollbarComponent;

    contentChanged: boolean;

    @Input()
    messages: Message[];

    constructor () { }

    ngOnChanges() {
        this.contentChanged = true;
    }

    ngAfterViewChecked(): void {
        if (this.contentChanged) {
            this.scrollToBottom();
            this.contentChanged = false;
        }
    }


    private scrollToBottom() {
        this.componentRef.directiveRef.scrollToBottom();
    }
}
