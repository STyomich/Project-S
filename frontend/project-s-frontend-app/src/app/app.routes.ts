import { Routes } from '@angular/router';
import { UserLoginForm } from './features/users/components/user-login-form/user-login-form';
import { HomeComponent } from './pages/home/home.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'login',
    component: UserLoginForm
  },
  {
    path: 'register',
    component: UserLoginForm // Can be replaced with UserRegisterForm later
  },
  {
    path: 'profile',
    component: UserLoginForm // Can be replaced with UserProfileComponent later
  },
  {
    path: 'settings',
    component: UserLoginForm // Can be replaced with UserSettingsComponent later
  },
  {
    path: 'about',
    component: HomeComponent // Can be replaced with AboutComponent later
  },
  {
    path: 'services',
    component: HomeComponent // Can be replaced with ServicesComponent later
  },
  {
    path: 'contact',
    component: HomeComponent // Can be replaced with ContactComponent later
  },
  {
    path: '**',
    redirectTo: ''
  }
];


