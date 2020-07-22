import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { UserManager, UserManagerSettings, User } from 'oidc-client';
import { BehaviorSubject, throwError } from 'rxjs'; 

//import { BaseService } from "../../shared/base.service";
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService
  //extends BaseService
{

  // Observable navItem source
  private _authNavStatusSource = new BehaviorSubject<boolean>(false);
  // Observable navItem stream
  authNavStatus$ = this._authNavStatusSource.asObservable();

  private manager = new UserManager(getClientSettings());
  public user: User | null;

  constructor(private http: HttpClient,private router: Router) { 
    //super();     
    
    this.manager.getUser().then(user => { 
      this.user = user;      
      this._authNavStatusSource.next(this.isAuthenticated());
    });
  }

  login() { 
    return this.manager.signinRedirect();   
  }

  completeAuthentication() {

    let manager = new UserManager({ response_mode: "query" })
      manager.signinRedirectCallback().then(user => {
        console.log(user);
        this.user = user;
        this._authNavStatusSource.next(this.isAuthenticated());  
        this.router.navigate(['/home']);    
    })
    .catch(error => {
        console.log(error);
    });

  //   this.manager.getUser().then(function (user) {
  //     if (user) {
  //       console.log("User logged in", user.profile);
  //     }
  //     else {
  //       console.log("User not logged in");
  //     }
  // });
          
  }  

  register(userRegistration: any) {    
    return this.http.post('https://localhost:44337/account', userRegistration).pipe(catchError(this.handleError));
  }

  isAuthenticated(): boolean {
    return this.user != null && !this.user.expired;
  }

  get authorizationHeaderValue(): string {
    return `${this.user.token_type} ${this.user.access_token}`;
  }

  get name(): string {
    return this.user != null ? this.user.profile.name : '';
  }

  async signout() {
    await this.manager.signoutRedirect();
  }

  api() {
    this.manager.getUser().then(function (user) {
        var url = "https://localhost:6001/identity";
  
        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            console.log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
  }


   handleError(error: any) {

    var applicationError = error.headers.get('Application-Error');

    // either application-error in header or model error in body
    if (applicationError) {
      return throwError(applicationError);
    }

    var modelStateErrors: string = '';

    // for now just concatenate the error descriptions, alternative we could simply pass the entire error response upstream
    for (var key in error.error) {
      if (error.error[key]) modelStateErrors += error.error[key].description + '\n';
    }

    modelStateErrors = modelStateErrors = '' ? null : modelStateErrors;
    return throwError(modelStateErrors || 'Server error');
  }
}



export function getClientSettings(): UserManagerSettings {
  return {
      authority: 'https://localhost:44337',
      client_id: 'angular_spa',
      redirect_uri: 'http://localhost:4200/auth-callback',
      post_logout_redirect_uri: 'http://localhost:4200/',
      response_type:"code",
      scope:"openid profile email api1",
      filterProtocolClaims: true,
      loadUserInfo: true,
      automaticSilentRenew: true,
      silent_redirect_uri: 'http://localhost:4200/silent-refresh.html'
  };
}
