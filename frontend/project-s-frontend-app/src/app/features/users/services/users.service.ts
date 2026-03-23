import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from '../../../core/services/cookie.service';
import { UserShortInfo } from '../models/user-short-info.model';
import { API_URL } from '../../../environments/environments.local';
import { UserCreateRequestDto } from '../models/user-create-request.dto';
import { UserLoginRequestDto } from '../models/user-login-request.dto';
import { UserLoginResponseDto } from '../models/user-login-response.dto';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private readonly TOKEN_KEY = 'auth_token';

  constructor(
    private http: HttpClient,
    private cookieService: CookieService
  ) {}

  getUserShortInfo() {
    return this.http.get<UserShortInfo[]>(API_URL + 'users/short-info');
  }

  create(dto: UserCreateRequestDto) {
    return this.http.post(API_URL + '/users', dto);
  }

  login(loginRequestDto: UserLoginRequestDto): Observable<UserLoginResponseDto> {
    return this.http.post<UserLoginResponseDto>(API_URL + '/login', loginRequestDto)
      .pipe(
        tap(response => this.storeToken(response.token))
      );
  }

  register(registerRequestDto: UserCreateRequestDto) {
    return this.http.post(API_URL + '/register', registerRequestDto);
  }

  private storeToken(token: string): void {
    this.cookieService.set(this.TOKEN_KEY, token, {
      path: '/',
      secure: true,
      sameSite: 'Strict'
    });
  }

  getToken(): string | null {
    return this.cookieService.get(this.TOKEN_KEY) || null;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  logout(): void {
    this.cookieService.delete(this.TOKEN_KEY);
  }
}
