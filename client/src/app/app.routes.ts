import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ChatComponent } from './components/chat/chat.component';
import { RoomCreationComponent } from './components/room-creation/room-creation.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'chat/:roomId', component: ChatComponent },
  { path: 'create-room', component: RoomCreationComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' }
];
