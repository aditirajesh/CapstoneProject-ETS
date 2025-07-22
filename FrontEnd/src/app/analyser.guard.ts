import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AuthService } from './services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AnalyserGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.authService.currentUser$.pipe(
      map(user => {
        if (user && user.role === 'Analyser') {
          return true; // Allow access if user is an Analyser
        }
        
        // If not an Analyser, redirect to their default home or login
        this.router.navigate(['/home']);
        return false;
      })
    );
  }
}