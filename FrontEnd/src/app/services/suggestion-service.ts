import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SuggestionAddDto, SuggestionDto } from '../models/SuggestionDTOs';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../environments/environment.prod';

@Injectable({
  providedIn: 'root'
})
export class SuggestionService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/suggestions`;
  

  constructor(private http: HttpClient) {}

  private formHeader(): HttpHeaders {
    const token = localStorage.getItem('expense_tracker_access_token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }


  addSuggestion(suggestion: SuggestionAddDto): Observable<SuggestionDto> {
    console.log(this.apiUrl);
    return this.http.post<SuggestionDto>(this.apiUrl, suggestion, { headers: this.formHeader() })
      .pipe(catchError(this.handleError));
  }


  getReceivedSuggestions(): Observable<SuggestionDto[]> {
    const url = `${this.apiUrl}/received`;
    return this.http.get<SuggestionDto[]>(url, { headers: this.formHeader() })
      .pipe(catchError(this.handleError));
  }


  markAsRead(suggestionId: number): Observable<void> {
    const url = `${this.apiUrl}/${suggestionId}/read`;
    return this.http.put<void>(url, {}, { headers: this.formHeader() })
      .pipe(catchError(this.handleError));
  }
  
  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('SuggestionService Error:', error);
    const errorMessage = error.error?.message || error.message || 'An unknown error occurred with the suggestion service.';
    return throwError(() => new Error(errorMessage));
  }
}