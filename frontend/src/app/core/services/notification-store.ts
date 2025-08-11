import { inject, Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Observable, Subject } from 'rxjs';
import {
  Notification,
  NotificationType,
} from '../../shared/components/toaster-notification/Notification';

@Injectable({
  providedIn: 'root',
})
export class NotificationStore {
  private readonly subject = new Subject<Notification>();
  private keepAfterRouteChange = true;

  router = inject(Router);
  public constructor() {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart) {
        if (this.keepAfterRouteChange) {
          this.keepAfterRouteChange = false;
        }
      }
    });
  }

  public getAlert(): Observable<Notification> {
    return this.subject.asObservable();
  }

  public success(
    message: string,
    keepAfterRouteChange = true,
    closeAfterTime = true,
  ) {
    this.showNotification(
      NotificationType.Success,
      message,
      keepAfterRouteChange,
      closeAfterTime,
    );
  }

  public error(
    message: string,
    keepAfterRouteChange = true,
    closeAfterTime = true,
  ) {
    this.showNotification(
      NotificationType.Error,
      message,
      keepAfterRouteChange,
      closeAfterTime,
    );
  }

  public info(
    message: string,
    keepAfterRouteChange = true,
    closeAfterTime = true,
  ) {
    this.showNotification(
      NotificationType.Info,
      message,
      keepAfterRouteChange,
      closeAfterTime,
    );
  }

  public warn(
    message: string,
    keepAfterRouteChange = true,
    closeAfterTime = true,
  ) {
    this.showNotification(
      NotificationType.Warning,
      message,
      keepAfterRouteChange,
      closeAfterTime,
    );
  }

  private showNotification(
    type: NotificationType,
    message: string,
    keepAfterRouteChange = false,
    closeAfterTime = true,
  ) {
    this.keepAfterRouteChange = keepAfterRouteChange;
    this.subject.next(new Notification(type, message, closeAfterTime));
  }
}
