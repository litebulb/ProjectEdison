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
import { AuthService } from './auth.service'

@Injectable({
  providedIn: 'root',
})
export class TokenInterceptorService implements HttpInterceptor, OnDestroy {
  private token: string
  private authTokenSub$: Subscription

  constructor(
    private store: Store<AppState>,
    private authService: AuthService
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
    const req = request.clone({
      setHeaders: {
        Authorization: `Bearer ${this.token}`,
      },
    })

    this.authService.refreshToken()

    return next.handle(req)
  }
}
