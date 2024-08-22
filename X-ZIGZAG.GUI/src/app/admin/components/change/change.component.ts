import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminService } from '../../services/admin.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ToastService } from '../../../toast.service';

@Component({
  selector: 'app-change',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './change.component.html',
  styleUrl: './change.component.scss'
})
export class ChangeAdminComponent implements OnInit{
  constructor(private router:Router,private route:ActivatedRoute,private adminService:AdminService,private toastService:ToastService){}
  AdminUsername:string|null=null;
  NewUsername!:string;
  NewPassword!:string;
  ErrorMessage:string ="";
  ngOnInit(): void {
    this.route.paramMap.subscribe((params)=>{
      this.AdminUsername=params.get("username");
      if(this.AdminUsername!=null){
        this.NewUsername=this.AdminUsername;
      }else{
        this.router.navigate(["/Admin"]);
      }
    })
  }
  UpdateUsername(){
    if(this.AdminUsername!=null){
      const updateReq= this.adminService.UpdateAdmin(this.AdminUsername,{username:this.NewUsername},"username");
      if(updateReq!=null){
        updateReq.subscribe((response)=>{
          this.toastService.showToast("Username Updated!");
          this.router.navigate(["/Admin/Update/"+this.NewUsername]);
        },(error)=>{
          if (error.status == 401) {
            this.ErrorMessage = 'Try To Login Again';
            this.router.navigate(['/Logout']);
          } else if (error.status == 404) {
            this.ErrorMessage = 'Not Found';
            this.router.navigate(['/Admin']);
          }
          else if (error.status == 409) {
            this.ErrorMessage = 'Someone with this username already exist';
          } else {
            this.ErrorMessage = 'Something went wrong....';
          }
        })
      }
    }else{
      this.router.navigate(["/Admin"]);
    }
  }
  UpdatePassword(){
    if(this.AdminUsername!=null){
      const updateReq= this.adminService.UpdateAdmin(this.AdminUsername,{password:this.NewPassword},"password");
      if(updateReq!=null){
        updateReq.subscribe((response)=>{
          this.toastService.showToast("Password Updated!");
          this.router.navigate(["/Admin/Update/"+this.NewUsername]);
        },(error)=>{
          if (error.status == 401) {
            this.ErrorMessage = 'Try To Login Again';
            this.router.navigate(['/Logout']);
          } else if (error.status == 404) {
            this.ErrorMessage = 'Not Found';
            this.router.navigate(['/Admin']);
          } else {
            this.ErrorMessage = 'Something went wrong....';
          }
        })
      }
    }else{
      this.router.navigate(["/Admin"]);
    }
  }
}
