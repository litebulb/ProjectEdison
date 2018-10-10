import { Component, ChangeDetectionStrategy } from '@angular/core';
import { AuthService } from '../../../../shared/services/auth.service';

@Component({
  selector: 'app-topnav',
  templateUrl: './topnav.component.html',
  styleUrls: ['./topnav.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TopnavComponent {

  constructor(private authService: AuthService) { }

  logout() {
    this.authService.logout();
  }

}
