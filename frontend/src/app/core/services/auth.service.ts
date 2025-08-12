import { Injectable } from '@angular/core';
import Keycloak from 'keycloak-js';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private keycloak = new Keycloak({
    url: 'http://localhost:8080',
    realm: 'sumisan',
    clientId: 'wash-frontend',
  });

  init(): Promise<boolean> {
    return this.keycloak.init({ onLoad: 'login-required' });
  }

  login(): void {
    this.keycloak.login();
  }

  logout(): void {
    this.keycloak.logout();
  }

  getToken(): string | undefined {
    return this.keycloak.token;
  }

  isAuthenticated(): boolean {
    return !!this.keycloak.token;
  }

  hasRole(role: string): boolean {
    return this.keycloak.hasRealmRole(role);
  }
}
