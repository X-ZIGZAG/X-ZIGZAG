import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../services/admin.service';
import { response } from 'express';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { CreateAdminComponent } from "../create/create.component";

@Component({
  selector: 'app-list',
  standalone: true,
  imports: [CommonModule, RouterModule, CreateAdminComponent],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss'
})
export class ListAdminComponent implements OnInit {
  ErrorMessage:string="";
  Admins:string[]=[];
  constructor(private adminService:AdminService,private router:Router){}
  
  ngOnInit(): void {
    this.LoadData();
  }
  LoadData(){
    const listAdminReq= this.adminService.getAllAdmins();
    if(listAdminReq!=null) {
      listAdminReq.subscribe(
        (response) => {
          this.Admins=response;
        },
        (error) => {
          if (error.status == 401) {
            this.ErrorMessage = 'Try To Login Again';
            this.router.navigate(['/Logout']);
          } else {
            this.ErrorMessage = 'Something went wrong....';
          }
        }
      );
    }
  }
  deleteUser(username:string){
    const DeleteUserReq= this.adminService.DeleteAdmin(username);
    if(DeleteUserReq!=null) {
      DeleteUserReq.subscribe(
        (response) => {
          window.location.reload();
        },
        (error) => {
          if (error.status == 401) {
            this.ErrorMessage = 'Try To Login Again';
            this.router.navigate(['/Logout']);
          } else if (error.status == 404) {
            this.ErrorMessage = 'Not Found';
            window.location.reload();
          } else {
            this.ErrorMessage = 'Something went wrong....';
          }
        }
      );
    }
  }  
}
