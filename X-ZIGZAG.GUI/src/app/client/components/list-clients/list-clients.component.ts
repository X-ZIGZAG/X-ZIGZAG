import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ClientService } from '../../services/client.service';
import { Router, RouterModule } from '@angular/router';
import { ClientInfo } from '../../models/clientInfo.module';

@Component({
  selector: 'app-list-clients',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule], // Add RouterModule here
  templateUrl: './list-clients.component.html',
  styleUrls: ['./list-clients.component.scss']
})
export class ListClientsComponent implements OnInit {
  clients:ClientInfo[] = []
  Checking:boolean=false;
  showErrorMessage:boolean= false;
  ErrorMessage:string='';
  constructor(private clientService:ClientService, private router: Router){}
  ngOnInit(): void {
    this.Checking=true;
    const clientsRequest= this.clientService.getAllClients();
    if(clientsRequest!=null){
      clientsRequest.subscribe(
        (response)=>{
        this.showErrorMessage=false;
         this.clients=response;
      },(error)=>{
        if(error.status==401){
          this.ErrorMessage="Try To Login Again";
          this.router.navigate(['/Logout']);
        }else{
          this.ErrorMessage="Something went wrong....";
        }
        this.showErrorMessage=true;
      }
      );
    }
    this.Checking=false;
  }
  OpenClient(clientId:string){
    this.router.navigate(["Client/"+clientId]);
  }
}
