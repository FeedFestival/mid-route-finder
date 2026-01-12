import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Unity } from './Unity/unity';
import { PwaInstallService } from './Unity/pwa-install.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Unity],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = signal('mid-route-finder-ui');

  private readonly pwa = inject(PwaInstallService);

  installPwa() {
    this.pwa.promptInstall();
  }
}
