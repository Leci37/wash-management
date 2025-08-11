import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TitleStore {
  private privTitle = signal('');
  title = this.privTitle.asReadonly();

  public updateTitle(title: string) {
    this.privTitle.set(title);
  }
}
