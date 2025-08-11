import { Routes } from '@angular/router';
import { Shell } from './modules/shell/pages/shell';
import { Inicio } from './modules/inicio/pages/inicio/inicio';
import { Nuevo } from './modules/nuevo/pages/nuevo/nuevo';
import { Finalizar } from './modules/finalizar/pages/finalizar/finalizar';
import { FinalizarMenu } from './modules/finalizar/pages/finalizar-menu/finalizar-menu';
import { Perfil } from './modules/perfil/pages/perfil/perfil';
import { confirmSavedChanges } from './core/guard/unsavedChanges';
import { Buscar } from './modules/buscar/pages/buscar/buscar';
import { BuscarList } from './modules/buscar/pages/buscar-list/buscar-list';
import { ApiTest } from './modules/api-test/api-test';
import { AuthGuard } from './core/guard/auth.guard';
import { RoleGuard } from './core/guard/role.guard';

export enum NavigationRoutes {
  Inicio = 'inicio',
  Buscar = 'buscar',
  Buscar_List = 'buscar-list',
  Nuevo = 'nuevo',
  Finalizar = 'finalizar',
  Finalizar_1 = 'finalizar/1',
  Finalizar_2 = 'finalizar/2',
  Perfil = 'perfil',
}
export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./modules/auth/login/login.component').then(
        (m) => m.LoginComponent,
      ),
  },
  { path: 'api-test', component: ApiTest },
  {
    path: '',
    component: Shell,
    canActivate: [AuthGuard],
    children: [
      { path: NavigationRoutes.Inicio, component: Inicio },
      { path: NavigationRoutes.Buscar, component: Buscar },
      { path: NavigationRoutes.Buscar_List, component: BuscarList },
      {
        path: NavigationRoutes.Nuevo,
        component: Nuevo,
        canDeactivate: [confirmSavedChanges],
      },
      { path: NavigationRoutes.Finalizar, component: FinalizarMenu },
      {
        path: `${NavigationRoutes.Finalizar}/:maq`,
        component: Finalizar,
        canDeactivate: [confirmSavedChanges],
      },
      { path: NavigationRoutes.Perfil, component: Perfil },
    ],
  },
  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./shared/components/unauthorized/unauthorized.component').then(
        (m) => m.UnauthorizedComponent,
      ),
  },
  { path: '**', redirectTo: 'login' },
];
