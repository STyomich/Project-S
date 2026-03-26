import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from '../../../core/services/cookie.service';
import { UserShortInfo } from '../models/user-short-info.model';
import { API_URL } from '../../../environments/environments.local';
import { UserCreateRequestDto } from '../models/user-create-request.dto';
import { UserLoginRequestDto } from '../models/user-login-request.dto';
import { UserLoginResponseDto } from '../models/user-login-response.dto';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { tap, shareReplay, switchMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private readonly TOKEN_KEY = 'auth_token';
  private currentUser$ = new BehaviorSubject<UserShortInfo | null>(null);
  private userLoaded = false;

  constructor(
    private http: HttpClient,
    private cookieService: CookieService,
  ) {}

  // Fetch all users short info
  getUserShortInfo() {
    return this.http.get<UserShortInfo>(API_URL + '/users/short-info');
  }

  // Get current logged-in user (observable)
  getCurrentUser(): Observable<UserShortInfo | null> {
    return this.currentUser$.asObservable();
  }

  // Get current user snapshot
  getCurrentUserSnapshot(): UserShortInfo | null {
    return this.currentUser$.value;
  }

  // Load current user from API (call this on app init if logged in)
  loadCurrentUser(): Observable<UserShortInfo> {
    if (this.userLoaded && this.currentUser$.value) {
      return of(this.currentUser$.value);
    }

    return this.http.get<UserShortInfo>(API_URL + '/users/current').pipe(
      tap((user) => {
        this.currentUser$.next(user);
        this.userLoaded = true;
        console.log('Current user loaded:', user);
      }),
      shareReplay(1), // Cache the request
    );
  }

  create(dto: UserCreateRequestDto) {
    return this.http.post(API_URL + '/users', dto);
  }

  login(loginRequestDto: UserLoginRequestDto): Observable<UserLoginResponseDto> {
    return this.http.post<UserLoginResponseDto>(API_URL + '/users/login', loginRequestDto).pipe(
      tap((response) => this.storeToken(response.token)),
      switchMap((response) =>
        this.loadCurrentUser().pipe(
          switchMap(() => of(response)), // Return the original login response
        ),
      ),
    );
  }

  register(registerRequestDto: UserCreateRequestDto) {
    return this.http.post(API_URL + '/users/register', registerRequestDto);
  }

  private storeToken(token: string): void {
    this.cookieService.set(this.TOKEN_KEY, token, {
      path: '/',
      secure: true,
      sameSite: 'Strict',
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
    this.currentUser$.next(null);
    this.userLoaded = false;
  }
}
