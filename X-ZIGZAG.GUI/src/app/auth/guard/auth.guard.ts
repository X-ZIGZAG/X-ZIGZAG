import { CanActivateFn ,Router} from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  if(authService.getToken()!=null){
    return true
  }
  router.navigate(['/']);
  return false;
};
