import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RoomService } from '../../services/room.service';
import { Room } from '../../models/room.interface';

@Component({
  selector: 'app-room-creation',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './room-creation.component.html',
  styleUrls: ['./room-creation.component.scss']
})
export class RoomCreationComponent implements OnInit {
  roomName: string = '';
  error: string = '';
  rooms: Room[] = [];
  isLoading: boolean = true;

  constructor(
    private roomService: RoomService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadRooms();
  }

  loadRooms() {
    this.isLoading = true;
    this.roomService.getRooms().subscribe({
      next: (rooms) => {
        this.rooms = rooms;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Failed to load rooms. Please try again.';
        this.isLoading = false;
        console.error('Error loading rooms:', err);
      }
    });
  }

  createRoom() {
    if (!this.roomName.trim()) {
      this.error = 'Room name is required';
      return;
    }

    this.roomService.createRoom(this.roomName).subscribe({
      next: (room) => {
        this.rooms.unshift(room);
        this.roomName = '';
        this.error = '';
      },
      error: (err) => {
        this.error = 'Failed to create room. Please try again.';
        console.error('Error creating room:', err);
      }
    });
  }

  joinRoom(roomId: string) {
    this.router.navigate(['/chat', roomId]);
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleString();
  }
} 