import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from '../../auth/services/auth.service';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Admin } from '../models/admin.module';
@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private token = this.authService.getToken();
  EndPoint:string = environment.apiUrl+"Admin";
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': ''+this.token
    })
  };
  constructor(private router:Router,private httpClient:HttpClient,private authService:AuthService) {}
  getAllAdmins():Observable<string[]>|null  {
    if(this.token==null)  {
      this.router.navigate(["/"]);
      return null;
    }else {
      return this.httpClient.get<string[]>(this.EndPoint,this.httpOptions);
    }
  }
  CreateAdmin(data:Admin):Observable<any>|null  {
    if(this.token==null)  {
      this.router.navigate(["/"]);
      return null;
    }else {
      return this.httpClient.post<any>(this.EndPoint,data,this.httpOptions);
    }
  }
  DeleteAdmin(username:string):Observable<any>|null  {
    if(this.token==null)  {
      this.router.navigate(["/"]);
      return null;
    }else {
      return this.httpClient.delete<any>(this.EndPoint+"/"+username,this.httpOptions);
    }
  }
  UpdateAdmin(username:string,data:any,update:string):Observable<any>|null  {
    if(this.token==null)  {
      this.router.navigate(["/"]);
      return null;
    }else {
      return this.httpClient.patch<any>(this.EndPoint+"/"+username+"/"+update,data,this.httpOptions);
    }
  }
}