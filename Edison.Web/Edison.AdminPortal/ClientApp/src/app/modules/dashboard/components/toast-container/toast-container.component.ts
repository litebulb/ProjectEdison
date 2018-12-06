import { ToastContainerDirective, ToastrService } from 'ngx-toastr';

import { Component, OnInit, ViewChild } from '@angular/core';

@Component({
    selector: 'app-toast-container',
    templateUrl: './toast-container.component.html',
    styleUrls: [ './toast-container.component.scss' ]
})
export class ToastContainerComponent implements OnInit {

    @ViewChild(ToastContainerDirective) toastContainer: ToastContainerDirective;

    constructor (private toastrService: ToastrService) { }

    ngOnInit() {
        this.toastrService.overlayContainer = this.toastContainer;
    }

}
