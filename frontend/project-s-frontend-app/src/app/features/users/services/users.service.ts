import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserShortInfo } from '../models/user-short-info.model';
import { API_URL } from '../../../environments/environments.local';
import { UserCreateRequestDto } from '../models/user-create-request.dto';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  constructor(private http: HttpClient) {}

  getUserShortInfo() {
    return this.http.get<UserShortInfo[]>(API_URL + 'users/short-info');
  }

  create(dto: UserCreateRequestDto) {
    return this.http.post(API_URL + 'users', dto);
  }
}
