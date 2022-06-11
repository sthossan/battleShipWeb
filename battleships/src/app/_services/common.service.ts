import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, catchError, lastValueFrom, map, Observable, throwError } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CommonService {
    public currentUserToken: BehaviorSubject<any> | undefined;

    constructor(private http: HttpClient) { }


    getAsync(url: string): Observable<any> {
        return this.http.get<any>(environment.apiUrl + url).pipe(
            map(response => {
                return response;
            }),
            catchError(this.handleError)
        );
    }

    postAsync(url: string, body: any): Observable<any> {
        return this.http.post<any>(environment.apiUrl + url, body).pipe(
            map(response => {
                return response;
            }),
            catchError(this.handleError)
        );
    }

    private handleError(err: HttpErrorResponse) {
        let errorMessage = '';
        if (err.error instanceof ErrorEvent) {
            errorMessage = `An error occurred: ${err.error.message}`;
        } else {
            errorMessage = `Server returned code: ${err.status}, error message is: ${err.message}`;
        }
        console.error(errorMessage);
        return throwError(() => new Error(errorMessage));
    }


}
