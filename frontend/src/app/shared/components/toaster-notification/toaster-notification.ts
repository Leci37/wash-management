import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { NotificationStore } from '../../../core/services/notification-store';
import { Notification, NotificationType } from './Notification';
import { MatButtonModule } from '@angular/material/button';

const TOASTER_NOTIFICATION_TIME = 5000;
@Component({
  selector: 'cm-toaster-notification',
  imports: [CommonModule, TranslateModule, MatButtonModule],
  templateUrl: './toaster-notification.html',
  styleUrl: './toaster-notification.scss',
})
export class ToasterNotification implements OnInit {
  private readonly notificationStore = inject(NotificationStore);
  private notifications: Notification[] = [];

  // public constructor(
  //   private readonly notificationService: NotificationService,
  // ) {}

  public get toasters(): Notification[] {
    return this.notifications;
  }

  public ngOnInit() {
    this.notificationStore
      .getAlert()
      .subscribe((alert: Notification | undefined) => {
        if (!alert) {
          return;
        }

        this.notifications.push(alert);
        if (alert.closeAfterTime) {
          setTimeout(() => {
            this.removeNotification(alert);
          }, TOASTER_NOTIFICATION_TIME);
        }
      });
  }

  public removeNotification(notification: Notification) {
    this.notifications = this.notifications.filter((x) => x !== notification);
  }

  public cssClass(notification: Notification) {
    switch (notification.type) {
      case NotificationType.Success:
        return 'success';
      case NotificationType.Error:
        return 'error';
      case NotificationType.Info:
        return 'info';
      case NotificationType.Warning:
        return 'warn';
    }
  }
}
