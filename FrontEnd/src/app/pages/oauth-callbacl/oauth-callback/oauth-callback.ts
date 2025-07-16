import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-oauth-callback',
  imports: [],
  templateUrl: './oauth-callback.html',
  styleUrl: './oauth-callback.css'
})
export class OauthCallback implements OnInit{
  
  constructor(private route:ActivatedRoute,private router:Router,private auth:AuthService){}
  ngOnInit(): void {
    // this.auth.setSignInType('google');
    sessionStorage.setItem('signInType','google');
    const username= this.route.snapshot.queryParamMap.get('username') || '';
    const accessToken = this.route.snapshot.queryParamMap.get('accessToken');
    const refreshToken = this.route.snapshot.queryParamMap.get('refreshToken');
    const picture = this.route.snapshot.queryParamMap.get('picture') || '';
    console.log(picture)
    const expiryStr = Number(this.route.snapshot.queryParamMap.get('expiry'))*60*1000;
    console.log(expiryStr);
    const expiresAt = Date.now() + expiryStr;

    if (accessToken && refreshToken) {
      sessionStorage.setItem('username',username);
      sessionStorage.setItem('accessToken', accessToken);
      sessionStorage.setItem('refreshToken', refreshToken);
      sessionStorage.setItem('Role','User');
      sessionStorage.setItem('tokenExpiry',expiresAt.toString());
      // this.auth.startTokenTimer(expiryStr);

      if(picture)
        sessionStorage.setItem('picture',picture);
      // this.auth.setUser(username);
      // this.auth.setRole('Voter');
      // this.auth.setPicture(picture);
            alert('OAuth login succeded');

      setTimeout(() => {
          this.router.navigate(['/home']);
        }, 1000);
    } 
    else {
      alert('OAuth login failed');
      this.router.navigate(['/']);
    }
  }

}
