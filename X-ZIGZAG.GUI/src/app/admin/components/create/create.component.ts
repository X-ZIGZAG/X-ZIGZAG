import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../services/admin.service';
import { Router } from '@angular/router';
import { ToastService } from '../../../toast.service';

@Component({
  selector: 'app-admin-create',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateAdminComponent {
  Username!:string;
  ErrorMessage:string="";
  Password!:string;
  @Output() ReloadData = new EventEmitter<void>();
  constructor(private adminService:AdminService,private router:Router,private toastService:ToastService){}
  CreateUser(){
    const createUserReq=this.adminService.CreateAdmin({username:this.Username,password:this.Password});
    if(createUserReq!=null){
      createUserReq.subscribe((response)=>{
        this.ReloadData.emit();
        this.Username="";
        this.Password="";
        this.toastService.showToast("New Admin Has been created!")
       // window.location.reload();
      },(error)=>{
        if (error.status == 401) {
          this.ErrorMessage = 'Try To Login Again';
          this.router.navigate(['/Logout']);
        } else if (error.status == 400) {
          this.ErrorMessage = 'Bad Request';
        } else if (error.status == 409) {
          this.ErrorMessage = 'Admin with this username already exist';
        }else {
          this.ErrorMessage = 'Something went wrong....';
        }
      })
    }
  }
}
