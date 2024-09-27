import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ClientService } from "../../../../services/client.service";
import { Router } from "@angular/router";
import { ToastService } from "../../../../../toast.service";
import { MonacoEditorModule } from "ngx-monaco-editor-v2";
import { MonacoEditorConfigModule } from "../../../../../monaco-editor-config";
import { environment } from "../../../../../../environments/environment";
import { response } from "express";

@Component({
  selector: "app-create-instruction",
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MonacoEditorModule,
    MonacoEditorConfigModule,
  ],
  templateUrl: "./create-instruction.component.html",
  styleUrl: "./create-instruction.component.scss",
})
export class CreateInstructionComponent {
  editorOptions = { theme: "vs-dark", language: "csharp" };

  ErrorMessage: string = "";

  selectedOption: number = 0;

  firstInputPlaceHolder: string = "File Upload Path";
  secondInputPlaceHolder: string = "Download Link";

  firstInputValue: string = "";
  secondInputValue: string = "";

  @Input()
  ClientId!: string | null;

  @Output() ReloadData = new EventEmitter<void>();

  constructor(
    private clientService: ClientService,
    private router: Router,
    private toastService: ToastService
  ) {}

  onOptionChange(event: any) {
    this.ErrorMessage = "";
    this.firstInputValue = "";
    this.secondInputValue = "";
    this.selectedOption = Number(event.target.value);
    switch (this.selectedOption) {
      case 0: {
        this.firstInputPlaceHolder = "File To Upload Path";
        break;
      }
      case 1: {
        this.firstInputPlaceHolder = "File Download Destination";
        break;
      }
      case 2: {
        this.firstInputPlaceHolder = "CMD Command";
        break;
      }
      case 3: {
        this.firstInputPlaceHolder = "PowerShell Command";
        break;
      }
      case 9: {
        this.firstInputPlaceHolder = "C# Code";
        this.firstInputValue = `using System.Threading.Tasks;
public class Script
{
  public static async Task<string> ExecuteAsync{
      // 1. Make Sure There's A Class With the name Script
      // 2. The Function that will be executed must be named "Execute" Similiar To Main Function
      // 3. Everything Else Is the same

      // return "output";
  }
}`;
        this.editorOptions = { theme: "vs-dark", language: "csharp" };

        break;
      }
      case 10: {
        this.firstInputPlaceHolder = "VB Code";
        this.editorOptions = { theme: "vs-dark", language: "vb" };
        this.firstInputValue = `Public Class Script
    Public Sub Execute()
        '    1. Make Sure There's A Class With name Script
        '    2. The Function that will be executed must be named "Execute" Similiar To Main Function
        '    3. Everything Else Is the same
    End Sub
End Class`;
        break;
      }
    }
  }
  CreateInst() {
    this.ErrorMessage = "";
    var valueToSubmit = null;
    if (this.ClientId != null) {
      switch (this.selectedOption) {
        case 0: {
          valueToSubmit =
            this.firstInputValue;
          break;
        }
        case 1: {
          valueToSubmit =
            this.firstInputValue + "*.&-&.*" + this.secondInputValue;
          break;
        }
        case 2: {
          valueToSubmit = this.firstInputValue;
          break;
        }
        case 3: {
          valueToSubmit = this.firstInputValue;
          break;
        }
        case 9: {
          valueToSubmit = this.firstInputValue;
          break;
        }
        case 10: {
          valueToSubmit = this.firstInputValue;
          break;
        }
      }
      const createInst = this.clientService.CreateClientAction(this.ClientId, {
        code: this.selectedOption,
        functionArgs: valueToSubmit,
      });
      if (createInst != null) {
        createInst.subscribe(
          (response) => {
            this.ReloadData.emit();
            this.toastService.showToast("Created Successfully!");
          },
          (error) => {
            if (error.status == 401) {
              this.ErrorMessage = "Try To Login Again";
              this.router.navigate(["/Logout"]);
            } else if (error.status == 404) {
              this.ErrorMessage = "Not Found";
              this.router.navigate(["/Client"]);
            } else {
              this.ErrorMessage = "Something went wrong....";
            }
          }
        );
      }
      this.firstInputValue = "";
      this.secondInputValue = "";
    }
  }
}
