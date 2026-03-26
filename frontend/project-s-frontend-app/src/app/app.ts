import { Component, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from './layout/components/navbar/navbar';
import { UsersService } from './features/users/services/users.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  constructor(private usersService: UsersService) {}

  ngOnInit(): void {
    if (this.usersService.isLoggedIn()) {
      this.usersService.loadCurrentUser().subscribe();
    }
  }
}
