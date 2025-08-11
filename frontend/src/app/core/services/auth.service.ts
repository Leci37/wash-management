import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userId: number;
  userName: string;
  role: string;
  expiresAt: string;
  isAuthenticated: boolean;
}

export interface UserProfile {
  userId: number;
  userName: string;
  role: string;
  isActive: boolean;
  lastLogin?: string;
  permissions: string[];
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'https://localhost:7001/api/auth';
  private currentUserSubject = new BehaviorSubject<UserProfile | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    this.initializeAuth();
  }

  private initializeAuth(): void {
    const token = this.getToken();
    if (token && !this.isTokenExpired(token)) {
      this.loadCurrentUser();
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap((response) => {
        if (response.isAuthenticated) {
          localStorage.setItem('auth_token', response.token);
          localStorage.setItem(
            'user_data',
            JSON.stringify({
              userId: response.userId,
              userName: response.userName,
              role: response.role,
              expiresAt: response.expiresAt,
            })
          );
          this.loadCurrentUser();
        }
      })
    );
  }

  logout(): void {
    this.http.post(`${this.apiUrl}/logout`, {}).subscribe();
    localStorage.removeItem('auth_token');
    localStorage.removeItem('user_data');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  private loadCurrentUser(): void {
    this.http.get<UserProfile>(`${this.apiUrl}/profile`).subscribe({
      next: (user) => this.currentUserSubject.next(user),
      error: () => this.logout(),
    });
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    return token !== null && !this.isTokenExpired(token);
  }

  private isTokenExpired(token: string): boolean {
    try {
      const tokenData = JSON.parse(atob(token.split('.')[1]));
      const expiry = tokenData.exp * 1000;
      return Date.now() > expiry;
    } catch {
      return true;
    }
  }

  hasRole(role: string): boolean {
    const user = this.currentUserSubject.value;
    return user?.role === role;
  }

  hasAnyRole(roles: string[]): boolean {
    const user = this.currentUserSubject.value;
    return user ? roles.includes(user.role) : false;
  }

  hasPermission(permission: string): boolean {
    const user = this.currentUserSubject.value;
    return user?.permissions.includes(permission) ?? false;
  }

  getCurrentUser(): UserProfile | null {
    return this.currentUserSubject.value;
  }
}
