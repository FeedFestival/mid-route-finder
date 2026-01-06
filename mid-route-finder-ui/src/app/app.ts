import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Unity } from './Unity/unity';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Unity],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = signal('mid-route-finder-ui');

  clicked(): void {
    console.log('clicked: ', new Date().toISOString());
  }
}
