import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ClientService } from '../../services/client.service';
import { ClientInfo } from '../../models/clientInfo.module';
import { ClientSettings } from '../../models/clientSettings.module';
import { CommonModule } from '@angular/common';
import { error } from 'console';
import { FormsModule } from '@angular/forms';
import { BrowserComponent } from "./browser/browser.component";
import { ScreenshotsComponent } from "./screenshots/screenshots.component";
import { InstructionsComponent } from "./instructions/instructions.component";
import { ResultsComponent } from "./results/results.component";
import { ToastService } from '../../../toast.service';

@Component({
  selector: 'app-get-client',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, BrowserComponent, ScreenshotsComponent, InstructionsComponent, ResultsComponent],
  templateUrl: './get-client.component.html',
  styleUrl: './get-client.component.scss',
})
export class GetClientComponent implements OnInit {
  client: ClientInfo | null = null;

  ClientId: string | null = '';
  Checking: boolean = false;
  ErrorMessage: string = '';
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private clientService: ClientService,
    private toastService:ToastService
  ) {}
  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.ClientId = params.get('id');
      if (this.ClientId == null) {
        this.router.navigate(['/Client']);
      } else {
        const clientsRequest = this.clientService.getClient(this.ClientId);
        if (clientsRequest != null) {
          clientsRequest.subscribe(
            (response) => {
              this.client = response;
              this.clientService.getIpInfo(this.client.ipAddress).subscribe((ipdata)=>{
  
                  if(this.client!=null){
                    this.client.location= ipdata;
                  }
                
              },(error)=>{
              });
            },
            (error) => {
              if (error.status == 401) {
                this.ErrorMessage = 'Try To Login Again';
                this.router.navigate(['/Logout']);
              } else if (error.status == 404) {
                this.ErrorMessage = 'Not Found';
                this.router.navigate(['/Client']);
              } else {
                this.ErrorMessage = 'Something went wrong....';
              }
            }
          );
        }
      }
    });
  }

  Settings() {
    if(this.isModalOpen){
      this.isModalOpen = false;
    }else{
      this.isModalOpen = true;
    }
  }
  isModalOpen = false;

  currentPage:string="";
  NavigateTo(to:string){
    this.currentPage=to;
  }

  onBackgroundClick(event: MouseEvent) {
    if (event.target === event.currentTarget) {
      this.isModalOpen = false;;
    }
  }
  UpdateRequest(){
    if(this.ClientId!=null){
      var updateReq =this.clientService.CreateClientAction(this.ClientId, { code: -2, functionArgs: "" });
      if(updateReq!=null){
        updateReq.subscribe((response)=>{
          this.toastService.showToast("Request Has Been Sent Successfully !")
          this.NavigateTo('instruction');
        },(error)=>{
          if (error.status == 401) {
            this.ErrorMessage = 'Try To Login Again';
            this.router.navigate(['/Logout']);
          } else if (error.status == 404) {
            this.ErrorMessage = 'Not Found';
            this.router.navigate(['/Client']);
          } else {
            this.ErrorMessage = 'Something went wrong....';
          }    
        })
      }
    }
  }
  UpdateDuration() {
    this.ErrorMessage="";
    const clientSettings: ClientSettings = {
      CheckDuration: this.client?.checkDuration,
      CustomName:this.client?.customName
    };
    if (this.ClientId != null) {
      const clientsRequest = this.clientService.updateSettings(
        this.ClientId,
        clientSettings
      );
      if (clientsRequest != null) {
        clientsRequest.subscribe((response)=>{
          this.isModalOpen = false;
          this.toastService.showToast("Opeartion Has Been Done Successfully !")

        },(error)=>{
          if (error.status == 401) {
            this.ErrorMessage = 'Try To Login Again';
            this.router.navigate(['/Logout']);
          } else if (error.status == 404) {
            this.ErrorMessage = 'Not Found';
            this.router.navigate(['/Client']);
          } else {
            this.ErrorMessage = 'Something went wrong....';
          }        
        })
      }
    }
  }
}
