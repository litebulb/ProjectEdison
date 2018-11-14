import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core'
import { AdalService } from './core/services/adal.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: [ './app.component.scss' ],
})
export class AppComponent implements OnInit {
    constructor (private authService: AdalService) { }

    ngOnInit() {
        if (!this.authService.loggedIn()) {
            this.authService.login();
        }
    }

    isLoggedIn() {
        return this.authService.loggedIn()
    }
}
