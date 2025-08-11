import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Spinner } from './shared/components/spinner/spinner';
import { ToasterNotification } from './shared/components/toaster-notification/toaster-notification';

@Component({
  selector: 'cm-root',
  imports: [RouterOutlet, Spinner, ToasterNotification],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected title = 'controlmat-desk';
}
