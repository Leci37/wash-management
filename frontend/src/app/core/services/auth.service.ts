import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, map } from 'rxjs';

interface LoginResponseDto {
  token: string;
  userId: number;
  userName?: string;
  role?: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenKey = 'token';
  private readonly roleKey = 'role';
  private readonly userIdKey = 'userId';

  constructor(private http: HttpClient) {}

  login(credentials: { userName: string; password: string }): Observable<void> {
    return this.http
      .post<LoginResponseDto>('/api/auth/login', credentials)
      .pipe(
        tap((res) => {
          localStorage.setItem(this.tokenKey, res.token);
          localStorage.setItem(this.roleKey, res.role || '');
          localStorage.setItem(this.userIdKey, res.userId.toString());
        }),
        map(() => void 0)
      );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.roleKey);
    localStorage.removeItem(this.userIdKey);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  hasRole(role: string): boolean {
    return localStorage.getItem(this.roleKey) === role;
  }
}

