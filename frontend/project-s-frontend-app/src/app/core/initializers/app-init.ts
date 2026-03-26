import { inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { UsersService } from '../../features/users/services/users.service';

export function initUser() {
  const usersService = inject(UsersService);

  if (usersService.isLoggedIn()) {
    return firstValueFrom(usersService.loadCurrentUser());
  }

  return Promise.resolve();
}