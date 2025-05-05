import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { Room } from '../models/room.interface';

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private apiUrl = 'https://localhost:44352/api/Room';

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

  getRooms(): Observable<Room[]> {
    return this.http.get<Room[]>(this.apiUrl).pipe(
      catchError(this.handleError.bind(this))
    );
  }

  createRoom(name: string): Observable<Room> {
    return this.http.post<Room>(this.apiUrl, { name }).pipe(
      catchError(this.handleError.bind(this))
    );
  }
} 