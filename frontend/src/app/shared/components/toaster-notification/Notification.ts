export class Notification {
  public constructor(
    public readonly type: NotificationType,
    public readonly message: string,
    public readonly closeAfterTime = true,
  ) {}
}

export enum NotificationType {
  Success,
  Error,
  Info,
  Warning,
}
