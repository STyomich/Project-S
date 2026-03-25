import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UsersService } from '../../../features/users/services/users.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar implements OnInit, OnDestroy {
  isLoggedIn = false;
  userName: string | null = null;
  isDropdownOpen = false;
  private destroy$ = new Subject<void>();

  constructor(private usersService: UsersService) {}

  ngOnInit(): void {
    this.checkAuthStatus();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  checkAuthStatus(): void {
    this.isLoggedIn = this.usersService.isLoggedIn();

    if (this.isLoggedIn) {
      // Subscribe to current user observable
      this.usersService
        .getCurrentUser()
        .pipe(takeUntil(this.destroy$))
        .subscribe((user) => {
          this.userName = user?.userName || 'User';
        });
    }
  }

  toggleDropdown(): void {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  closeDropdown(): void {
    this.isDropdownOpen = false;
  }

  logout(): void {
    this.usersService.logout();
    this.isLoggedIn = false;
    this.userName = null;
    this.isDropdownOpen = false;
    window.location.href = '/';
  }
}
