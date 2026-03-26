import { Routes } from '@angular/router';
import { UserLoginForm } from './features/users/components/user-login-form/user-login-form';
import { Home } from './pages/home/home';
import { UserRegisterForm } from './features/users/components/user-register-form/user-register-form';

export const routes: Routes = [
  {
    path: '',
    component: Home,
  },
  {
    path: 'login',
    component: UserLoginForm,
  },
  {
    path: 'register',
    component: UserRegisterForm, // Can be replaced with UserRegisterForm later
  },
  {
    path: 'profile',
    component: UserLoginForm, // Can be replaced with UserProfileComponent later
  },
  {
    path: 'settings',
    component: UserLoginForm, // Can be replaced with UserSettingsComponent later
  },
  {
    path: 'about',
    component: Home, // Can be replaced with AboutComponent later
  },
  {
    path: 'services',
    component: Home, // Can be replaced with ServicesComponent later
  },
  {
    path: 'contact',
    component: Home, // Can be replaced with ContactComponent later
  },
  {
    path: '**',
    redirectTo: '',
  },
];
