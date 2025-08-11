import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SpinnerStore {
  private showSpinner = signal(false);
  spinner = this.showSpinner.asReadonly();

  public show() {
    this.showSpinner.set(true);
  }

  public hide() {
    this.showSpinner.set(false);
  }
}
