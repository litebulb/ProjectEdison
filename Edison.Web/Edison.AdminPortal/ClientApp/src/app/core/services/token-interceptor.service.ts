import { Injectable, OnDestroy } from '@angular/core'
import {
    HttpInterceptor,
    HttpRequest,
    HttpHandler,
    HttpEvent,
} from '@angular/common/http'
import { Observable, Subscription } from 'rxjs'
import { Store, select } from '@ngrx/store'
import { AppState } from '../../reducers'
import { authTokenSelector } from '../../reducers/auth/auth.selectors'
import { MsalService } from './msal.service'
import { environment } from '../../../environments/environment';
import { AdalService } from './adal.service';

@Injectable({
    providedIn: 'root',
})
export class TokenInterceptorService implements HttpInterceptor, OnDestroy {
    private token: string
    private authTokenSub$: Subscription

    constructor (
        private store: Store<AppState>,
        private authService: AdalService
    ) {
        this.authTokenSub$ = store
            .pipe(select(authTokenSelector))
            .subscribe(token => (this.token = token))
    }

    ngOnDestroy() {
        this.authTokenSub$.unsubscribe()
    }

    intercept(
        request: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        if (environment.authorize) {
            // this.authService.refreshToken()

            const req = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${this.token}`,
                },
            })

            return next.handle(req)
        }

        return next.handle(request)
    }
}
