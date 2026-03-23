import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UsersService } from '../../../features/users/services/users.service';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar implements OnInit {
  isLoggedIn = false;
  userName: string | null = null;
  isDropdownOpen = false;

  constructor(private usersService: UsersService) {}

  ngOnInit(): void {
    this.checkAuthStatus();
  }

  checkAuthStatus(): void {
    this.isLoggedIn = this.usersService.isLoggedIn();
    // In a real app, you might fetch user info from the API or localStorage
    const storedUserName = localStorage.getItem('userName');
    this.userName = storedUserName || 'User';
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
    // Navigate to home or login page
    window.location.href = '/';
  }
}

