import { Observable, Subscription } from 'rxjs';

import {
    Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild
} from '@angular/core';

import { Message } from '../../../../reducers/chat/chat.model';

@Component({
    selector: 'app-all-users-chat',
    templateUrl: './all-users-chat.component.html',
    styleUrls: [ './all-users-chat.component.scss' ],
})
export class AllUsersChatComponent implements OnInit {
    @ViewChild('activateResponseButton') activateResponseButton: ElementRef;

    @Input() userCount: number;
    @Input() messages: Message[];

    @Output() activateResponseClick = new EventEmitter();
    @Output() newMessage = new EventEmitter<{ message: string, userId: string }>();
    @Output() closeClick = new EventEmitter();

    messagesSub$: Subscription;
    userCount$: Observable<number>;
    message: string;

    ngOnInit() {
        this.activateResponseButton.nativeElement.focus();
    }

    showActivateResponse() {
        this.activateResponseClick.emit();
    }

    onEnter(event) {
        if (event.keyCode !== 13) {
            return;
        }

        this.newMessage.emit({ message: this.message, userId: '*' })
        this.messages.push({
            name: 'YOU',
            text: this.message,
            role: 'admin',
            self: true,
        });
        this.message = '';


        event.preventDefault();
        return false;
    }

    close() {
        this.closeClick.emit();
    }
}
