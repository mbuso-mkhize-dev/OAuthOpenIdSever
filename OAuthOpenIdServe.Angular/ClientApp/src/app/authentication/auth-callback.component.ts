import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-auth-callback',
  templateUrl: './auth-callback.component.html'
})
export class AuthCallbackComponent implements OnInit {

  error: boolean;

  constructor(private authService: AuthService, private router: Router, private route: ActivatedRoute) {}

  ngOnInit() {
 
    // check for error
    if (this.route.snapshot.fragment !=  null) {
      
        if(this.route.snapshot.fragment.indexOf('error') >= 0) {
        this.error=true; 
        return;    
      }
    }
    
  this.authService.completeAuthentication(); 
  }
}
