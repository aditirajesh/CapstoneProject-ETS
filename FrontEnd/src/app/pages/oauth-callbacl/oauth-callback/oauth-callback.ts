import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { TokenStorageService } from '../../../services/tokenstorage.service';
import { JwtService } from '../../../services/jwt.service';
import { CurrentUser } from '../../../models/CurrentUser';
import { BehaviorSubject } from 'rxjs';
import { AuthState } from '../../../models/AuthState';

@Component({
  selector: 'app-oauth-callback',
  imports: [],
  templateUrl: './oauth-callback.html',
  styleUrl: './oauth-callback.css'
})
export class OauthCallback implements OnInit{
  
  private authStateSubject = new BehaviorSubject<AuthState>({
          isAuthenticated: false,
          user: null,
          accessToken: null,
          refreshToken: null,
          tokenExpiry: null
      });
   private navigateAfterLogin(user: CurrentUser | null): void {
        if (!user) {
            console.log('AuthService: No user provided for navigation');
            return;
        }
        
        console.log('AuthService: Navigating user based on role:', user.role);
        
        if (user.role === 'Admin') {
            console.log('AuthService: Redirecting admin to admin-home');
            this.router.navigate(['/admin-home']);
        } else {
            console.log('AuthService: Redirecting user to home');
            this.router.navigate(['/home']);
        }
    }
  private updateAuthState(
          isAuthenticated: boolean, 
          user: CurrentUser | null, 
          accessToken: string | null, 
          refreshToken: string | null
      ): void {
          const tokenExpiry = accessToken ? this.jwtService.getExpiration(accessToken) : null;
          
          this.authStateSubject.next({
              isAuthenticated,
              user,
              accessToken,
              refreshToken,
              tokenExpiry
          });
      }
  constructor(private route:ActivatedRoute,private router:Router,private tokenService:TokenStorageService,private jwtService: JwtService,){}
  ngOnInit(): void {
    // this.auth.setSignInType('google');
    // sessionStorage.setItem('signInType','google');
    const username= this.route.snapshot.queryParamMap.get('username') || '';
    const accessToken = this.route.snapshot.queryParamMap.get('accessToken');
    const refreshToken = this.route.snapshot.queryParamMap.get('refreshToken');
    // const picture = this.route.snapshot.queryParamMap.get('picture') || '';
    // console.log(picture)
    // const expiryStr = Number(this.route.snapshot.queryParamMap.get('expiry'))*60*1000;
    // console.log(expiryStr);
    // const expiresAt = Date.now() + expiryStr;
    if (accessToken && refreshToken) {
      const user = this.jwtService.extractUserFromToken(accessToken);
      this.tokenService.saveTokens(accessToken,refreshToken, user);
      this.updateAuthState(true, user, accessToken,refreshToken);
                    
      this.navigateAfterLogin(user);
                    
      console.log('AuthService: User session established');
    } else {
      console.error('AuthService: Login response missing tokens');
      throw new Error('Invalid login response');
    }


  }

}
