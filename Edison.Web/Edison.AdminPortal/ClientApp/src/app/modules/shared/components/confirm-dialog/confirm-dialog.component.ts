import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ConfirmDialogData } from '../../../../core/models/confirmDialogData';

@Component({
    selector: 'app-confirm-dialog',
    templateUrl: './confirm-dialog.component.html',
    styleUrls: [ './confirm-dialog.component.scss' ]
})
export class ConfirmDialogComponent implements OnInit {
    noButtonLabel: string = 'No';
    yesButtonLabel: string = 'Yes';
    defaultText: string = 'Are you sure?'

    constructor (
        public dialogRef: MatDialogRef<ConfirmDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogData) { }

    ngOnInit(): void {
        if (!this.data) { return; }

        if (this.data.noButtonLabel) {
            this.yesButtonLabel = this.data.noButtonLabel;
        }

        if (this.data.yesButtonLabel) {
            this.noButtonLabel = this.data.yesButtonLabel;
        }

        if (this.data.defaultText) {
            this.defaultText = this.data.defaultText;
        }
    }

    onYesClick() {
        this.dialogRef.close(true);
    }

    onNoClick(): void {
        this.dialogRef.close(false);
    }

}
