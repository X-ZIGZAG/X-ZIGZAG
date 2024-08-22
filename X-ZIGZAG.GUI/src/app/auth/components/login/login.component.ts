import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl,FormGroup,ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  showErrorMessage:boolean= false;
  ErrorMessage:string='';
  Checking:boolean=false;
  loginForm = new FormGroup({
    username : new FormControl(''),
    password : new FormControl('')
  });
  constructor(private authService:AuthService,private router:Router){
    if(this.authService.getToken()!=null){
      this.router.navigate(['/Client']);
    }
  }
    onLogin(){
    this.Checking=true;
    this.authService.Login(
      {
        username:this.loginForm.value.username??'',
        password:this.loginForm.value.password??''
      }).subscribe(
        (response)=>{
          this.showErrorMessage=false;
          this.authService.setToken(response.token);
          this.router.navigate(['/Client']);
      },(error)=>{
        if(error.status==401){
          this.ErrorMessage=error.error.message;
        }else{
          this.ErrorMessage="Something went wrong....";
        }
        this.showErrorMessage=true;
      }
      );
      this.Checking=false;
  }
}
