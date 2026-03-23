import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UsersService } from '../../services/users.service';
import { UserLoginRequestDto } from '../../models/user-login-request.dto';

@Component({
  selector: 'app-user-login-form',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './user-login-form.html',
  styleUrl: './user-login-form.css',
})
export class UserLoginForm implements OnInit {
  loginForm!: FormGroup;
  loading = false;
  error: string | null = null;
  success = false;

  constructor(
    private fb: FormBuilder,
    private usersService: UsersService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.error = 'Please fill in all required fields correctly';
      return;
    }

    this.loading = true;
    this.error = null;
    this.success = false;

    const loginData: UserLoginRequestDto = {
      email: this.loginForm.get('email')?.value,
      password: this.loginForm.get('password')?.value
    };

    this.usersService.login(loginData).subscribe({
      next: (response) => {
        this.loading = false;
        this.success = true;
        this.error = null;
        this.loginForm.reset();
        console.log('Login successful!', response);
        // Here you can navigate to another page or emit an event
        // this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.loading = false;
        this.success = false;
        this.error = error.error?.message || 'Login failed. Please check your credentials.';
        console.error('Login error:', error);
      }
    });
  }

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }
}
