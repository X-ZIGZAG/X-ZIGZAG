// src/app/toast/toast.component.ts
import { Component, OnInit } from '@angular/core';
import { ToastService } from '../toast.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports:[CommonModule],
  templateUrl: './toast.component.html',
  styleUrls: ['./toast.component.scss']
})
export class ToastComponent implements OnInit {
  message: string | null = null;

  constructor(private toastService: ToastService) {}

  ngOnInit(): void {
    this.toastService.toast$.subscribe(message => {
      this.message = message;
      setTimeout(() => this.message = null, 3000); // Toast disappears after 3 seconds
    });
  }
}
