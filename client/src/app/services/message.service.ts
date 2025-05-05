import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { Message } from '../models/message.interface';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private apiUrl = 'https://localhost:44352/api/Message';

  constructor(
    private http: HttpClient,
    private router: Router
  ) { }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 401) {
      localStorage.removeItem('token');
      this.router.navigate(['/login']);
    }
    return throwError(() => error);
  }

  getMessages(roomId: string): Observable<Message[]> {
    return this.http.get<Message[]>(`${this.apiUrl}/${roomId}`).pipe(
      catchError(this.handleError.bind(this))
    );
  }
} 