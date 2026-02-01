import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class PwaInstallService {
  private deferredPrompt: any = null;
  public canInstall = false;
  public isStandalone = false;

  constructor() {
    this.isStandalone =
      window.matchMedia('(display-mode: standalone)').matches ||
      (window.navigator as any).standalone === true;

    window.addEventListener('beforeinstallprompt', (event: any) => {
      event.preventDefault();
      this.deferredPrompt = event;
      this.canInstall = true;
    });

    window.addEventListener('appinstalled', () => {
      this.canInstall = false;
    });
  }

  async promptInstall() {
    if (!this.deferredPrompt) return;
    await this.deferredPrompt.prompt();
  }
}
