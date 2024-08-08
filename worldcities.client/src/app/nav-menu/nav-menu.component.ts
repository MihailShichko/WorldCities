import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from '../auth/auth-service.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit, OnDestroy {
    private destroySubject = new Subject();
    isLoggedIn: boolean = false;

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isAuthentificated();
  }

  ngOnDestroy(): void {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }

  constructor(private authService: AuthService, private router: Router) {
    this.authService.authStatus.pipe(takeUntil(this.destroySubject))
      .subscribe(
        result => { this.isLoggedIn = result; }
      )
  }

  onLogout(): void {
    this.authService.logout();
  }


}
