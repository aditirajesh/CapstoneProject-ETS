import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AuthService } from './services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleRedirectGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.authService.currentUser$.pipe(
      map((user) => {
        if (user) {
          console.log(user.role);
          if (user.role === 'Admin') {
            this.router.navigate(['/admin-home']);
            return false;
          }
          // CHANGE: Added Analyser role check
          else if (user.role === 'Analyser') { 
            this.router.navigate(['/analyser-home']); // Redirect to analyser dashboard
            return false;
          }
          else {
            return true;
          }
        } else {
          this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
          return false;
        }
      })
    );
  }
}