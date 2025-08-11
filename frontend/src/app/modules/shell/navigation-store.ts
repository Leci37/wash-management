import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { NavTabLink } from './nav-tab-links';
import { NavigationRoutes } from '../../app.routes';

@Injectable({
  providedIn: 'root',
})
export class NavigationStore {
  private router = inject(Router);

  goTo(path: string | NavTabLink) {
    path = typeof path === 'string' ? path : path.path;
    this.router.navigate([path]);
  }

  goToInicio() {
    this.router.navigate([NavigationRoutes.Inicio]);
  }
}
