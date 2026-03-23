import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterModule],
  template: `
    <div class="home-container">
      <div class="hero">
        <h1>Welcome to Project S</h1>
        <p>Your modern web application built with Angular</p>
        <div class="cta-buttons">
          <a routerLink="/login" class="btn btn-primary">Get Started</a>
          <a routerLink="/about" class="btn btn-secondary">Learn More</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .home-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: calc(100vh - 70px);
    }

    .hero {
      text-align: center;
      padding: 40px;
      max-width: 600px;
    }

    h1 {
      font-size: 48px;
      margin-bottom: 20px;
      color: #333;
    }

    p {
      font-size: 20px;
      color: #666;
      margin-bottom: 40px;
    }

    .cta-buttons {
      display: flex;
      gap: 20px;
      justify-content: center;
    }

    .btn {
      padding: 12px 30px;
      border-radius: 4px;
      text-decoration: none;
      font-weight: 600;
      transition: all 0.3s;
      border: none;
      cursor: pointer;
      font-size: 16px;
    }

    .btn-primary {
      background-color: #667eea;
      color: white;
    }

    .btn-primary:hover {
      background-color: #5568d3;
    }

    .btn-secondary {
      border: 2px solid #667eea;
      color: #667eea;
      background-color: transparent;
    }

    .btn-secondary:hover {
      background-color: #667eea;
      color: white;
    }
  `]
})
export class HomeComponent {}

