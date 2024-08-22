import { Routes } from '@angular/router';
import { LoginComponent } from './auth/components/login/login.component';
import { ListClientsComponent } from './client/components/list-clients/list-clients.component';
import { authGuard } from './auth/guard/auth.guard';
import { LogoutComponent } from './logout/logout.component';
import { GetClientComponent } from './client/components/get-client/get-client.component';
import { BrowserComponent } from './client/components/get-client/browser/browser.component';
import { ScreenshotsComponent } from './client/components/get-client/screenshots/screenshots.component';
import { InstructionsComponent } from './client/components/get-client/instructions/instructions.component';
import { ResultsComponent } from './client/components/get-client/results/results.component';
import { ListAdminComponent } from './admin/components/list/list.component';
import { ChangeAdminComponent } from './admin/components/change/change.component';

export const routes: Routes = [
    {path:"",component:LoginComponent},
    {path:"Client",component:ListClientsComponent,canActivate:[authGuard]},
    {path:"Client/:id",component:GetClientComponent,canActivate:[authGuard]},
    {path:"Client/Browser/:id",component:BrowserComponent,canActivate:[authGuard]},
    {path:"Client/Screenshots/:id",component:ScreenshotsComponent,canActivate:[authGuard]},
    {path:"Client/Instructions/:id",component:InstructionsComponent,canActivate:[authGuard]},
    {path:"Client/Results/:id",component:ResultsComponent,canActivate:[authGuard]},
    {path:"Admin",component:ListAdminComponent,canActivate:[authGuard]},
    {path:"Admin/New",component:ListAdminComponent,canActivate:[authGuard]},
    {path:"Admin/Update/:username",component:ChangeAdminComponent,canActivate:[authGuard]},
    {path:"Logout",component:LogoutComponent,canActivate:[authGuard]}
];
