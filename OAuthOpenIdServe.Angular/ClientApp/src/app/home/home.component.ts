import { Component } from '@angular/core';
import { AuthService } from '../authentication/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  constructor(private authService: AuthService) {
    this.displayUser();
  }
  user = {};
  

  displayUser() {
    this.user = this.authService.user;
  }
}
