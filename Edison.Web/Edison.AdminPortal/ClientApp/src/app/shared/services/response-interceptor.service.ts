import { Injectable, OnDestroy } from '@angular/core'
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpResponse,
  HttpErrorResponse,
} from '@angular/common/http'
import { Observable, Subscription } from 'rxjs'
import { AuthService } from './auth.service'
import { Store, select } from '@ngrx/store'
import { AppState } from '../../reducers'
import { authTokenSelector } from '../../reducers/auth/auth.selectors'
import { tap } from 'rxjs/operators'

@Injectable({
  providedIn: 'root',
})
export class ResponseInterceptorService implements HttpInterceptor, OnDestroy {
  private token: string
  private authTokenSub$: Subscription

  constructor(
    private authService: AuthService,
    private store: Store<AppState>
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
    return next.handle(request).pipe(
      tap(
        (event: HttpEvent<any>) => {
          if (event instanceof HttpResponse) {
          }
        },
        (err: any) => {
          if (err instanceof HttpErrorResponse) {
            if (err.status === 401) {
              this.authService.logout()
            }
          }
        }
      )
    )
  }
}
