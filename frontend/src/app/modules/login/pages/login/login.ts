import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import { AuthService } from '../../../../core/services/auth.service';
import { NotificationStore } from '../../../../core/services/notification-store';

@Component({
  selector: 'cm-login',
  templateUrl: './login.html',
  styleUrl: './login.scss',
  imports: [CommonModule, ReactiveFormsModule, MatInputModule, MatButtonModule],
})
export class Login {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private notification = inject(NotificationStore);

  form = this.fb.group({
    userName: ['', [Validators.required, Validators.maxLength(100)]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  loading = false;

  onSubmit() {
    if (this.form.invalid) {
      return;
    }
    this.loading = true;
    this.auth
      .login(this.form.value as { userName: string; password: string })
      .subscribe({
        next: () => {
          this.router.navigate(['']);
        },
        error: () => {
          this.notification.error('LOGIN.ERROR');
          this.loading = false;
        },
      });
  }
}

