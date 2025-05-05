import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

interface LoginResponse {
  token: string;
  success: boolean;
}

interface RegisterResponse {
  success: boolean;
  message?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
  private apiUrl = 'https://localhost:44352/api/Auth';

  constructor(
    private router: Router,
    private http: HttpClient
  ) { }

  register(name: string, email: string, password: string): Observable<boolean> {
    return this.http.post<RegisterResponse>(`${this.apiUrl}/Register`, { name, email, password })
      .pipe(
        map(response => response ? true : false
        )
      );
  }

  login(email: string, password: string): Observable<boolean> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/Login`, { email, password })
      .pipe(
        map(response => {
          if (response.token) {
            localStorage.setItem('token', response.token);
            this.isAuthenticatedSubject.next(true);
            return true;
          }
          return false;
        })
      );
  }

  logout(): void {
    localStorage.removeItem('token');
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/login']);
  }
} 