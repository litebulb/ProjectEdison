import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: [ './header.component.scss' ]
})
export class HeaderComponent implements OnInit {
    @Input() title: string;
    @Input() showSaveButton: boolean;
    @Input() saveButtonDisabled: boolean;

    @Output() saveButtonClick = new EventEmitter();

    constructor () { }

    onSaveClick() {
        if (!this.saveButtonDisabled) {
            this.saveButtonClick.emit();
        }
    }

    ngOnInit() {
    }

}
