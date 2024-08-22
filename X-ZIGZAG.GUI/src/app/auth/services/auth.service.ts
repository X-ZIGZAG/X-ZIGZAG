import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loginUrl = environment.apiUrl + 'Auth';
  constructor(private http:HttpClient) {}
  Login(data:any): Observable<any> {
    return this.http.post(this.loginUrl, data);
  }
  setToken(token:string) {
    const expirationDate = new Date();
    expirationDate.setDate(expirationDate.getDate() + 7);
    document.cookie=`token=${token};expires=${expirationDate.toUTCString()};path=/;`;
  }
  getToken():string|null{
    if (typeof document === 'undefined') {
      return null;
    }
    const cookies = document.cookie.split(";");
    for(let cookie of cookies){
      if(cookie.includes("token")){
        return cookie.split("=")[1];
      }
    }
    return null;
  }
  logout(): void {
    document.cookie = 'token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
  }
}
