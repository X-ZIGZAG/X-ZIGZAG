import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ClientService } from '../../../services/client.service';
import { Screens } from '../../../models/screenshots.module';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { ClientSettings } from '../../../models/clientSettings.module';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../../../toast.service';

@Component({
  selector: 'app-screenshots',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './screenshots.component.html',
  styleUrl: './screenshots.component.scss',
})
export class ScreenshotsComponent implements OnInit {
  imageUrl: SafeUrl | undefined;

  showSettings: boolean = false;

  screenShotsPreviewMap = new Map<string, SafeUrl>();
  ScreenSelection = new Map<number, boolean>();

  ScreensList: Screens = { screens: [] };
  ClientId: string | null = null;
  ErrorMessage: string = '';
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private clientService: ClientService,
    private sanitizer: DomSanitizer,
    private toastService:ToastService
  ) {}
  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
         this.ClientId = params.get('id');
          this.LoadScreenshots();
    });
  }
  screenshotUrl: SafeUrl | null = null;
  LoadScreenshots(){
      if (this.ClientId == null) {
        this.router.navigate(['/Client']);
      } else {
        const screenshotRequest = this.clientService.getClientScreenshots(
          this.ClientId
        );
        if (screenshotRequest != null) {
          screenshotRequest.subscribe(
            (response) => {
              this.ScreensList = response;
              if (response.screens != null) {
                for (let screen of response.screens) {
                  this.ScreenSelection.set(screen.id,false);
                  if (screen.screenshots != null) {
                    for (let screenshot of screen.screenshots) {
                      this.GetImagePreview(screen.id,screenshot.screenshotId);
                    }
                  }
                }
              }
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
  ShowImage(ScreenIndex: number, imagePath: string) {
    this.ErrorMessage = '';
    if (this.ClientId != null) {
      const getScreenshotReq = this.clientService.getClientScreenshot(
        this.ClientId,
        imagePath,
        ScreenIndex
      );
      if (getScreenshotReq != null) {
        getScreenshotReq.subscribe(
          (response: Blob) => {
            const unsafeUrl = URL.createObjectURL(response);
            this.screenshotUrl =
              this.sanitizer.bypassSecurityTrustUrl(unsafeUrl);
              this.isModalOpen=true;
          },
          (error) => {
            this.ErrorMessage = 'Error loading image';
          }
        );
      }
    }
  }

  currentScreenIndex :number = -1;
  ShowScreen(ScreenIndex:number){
    if(ScreenIndex!=this.currentScreenIndex){
      this.currentScreenIndex=ScreenIndex;
    }else{
      this.currentScreenIndex=-1;

    }
  }

  isModalOpen = false;

  
  onBackgroundClick(event: MouseEvent) {
    if (event.target === event.currentTarget) {
      this.isModalOpen = false;;
    }
  }
  onSettingsBackgroundClick(event: MouseEvent) {
    if (event.target === event.currentTarget) {
      this.showSettings = false;;
    }
  }

  GetImagePreview(ScreenIndex: number, imagePath: string) {
    this.ErrorMessage = '';
    if (this.ClientId != null) {
      const getScreenshotReq = this.clientService.getClientScreenshotPreview(
        this.ClientId,
        imagePath,
        ScreenIndex
      );
      if (getScreenshotReq != null) {
        getScreenshotReq.subscribe(
          (response: any) => {            
            const unsafeUrl = URL.createObjectURL(response);
            this.screenShotsPreviewMap.set(ScreenIndex+"-"+imagePath,this.sanitizer.bypassSecurityTrustUrl(unsafeUrl));
          },
          (error) => {
          }
        );
      }
    }
    return null;
  }
  Settings() {
    if (this.showSettings) {
      this.showSettings = false;
    } else {
      this.showSettings = true;
    }
  }
  DeleteAll() {
    if(this.ClientId!=null){
      const deleteAllSCRReq = this.clientService.deleteClientScreenshots(this.ClientId);
      if(deleteAllSCRReq!=null){
        deleteAllSCRReq.subscribe(
          (response: any) => {
            this.toastService.showToast("Opeartion Has Been Done Successfully !")
            this.LoadScreenshots();
          },
          (error) => {
            this.ErrorMessage = 'Error loading image';
          }
        );
      }
    }
  }
  OneScreenShot() {
    const clientSettings: ClientSettings = {
      Screenshot: -1,
    };
    this.UpdateDuration(clientSettings);

  }
  CustomDuration() {
    const clientSettings: ClientSettings = {
      Screenshot: this.ScreensList.duration,
    };
    this.UpdateDuration(clientSettings);
  }
  UpdateDuration(clientSettings: ClientSettings) {
    this.ErrorMessage = '';
    if (this.ClientId != null) {
      const clientsRequest = this.clientService.updateSettings(
        this.ClientId,
        clientSettings
      );
      if (clientsRequest != null) {
        clientsRequest.subscribe(
          (response) => {
            this.showSettings = false;
            this.toastService.showToast("Opeartion Has Been Done Successfully !")
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
}
