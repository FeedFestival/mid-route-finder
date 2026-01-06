import { AfterViewInit, Component, ElementRef, HostListener, ViewChild } from '@angular/core';

declare function createUnityInstance(
  canvas: HTMLCanvasElement,
  config: any,
  onProgress: (progress: number) => void
): Promise<any>;

@Component({
  selector: 'unity',
  templateUrl: './unity.html',
})
export class Unity implements AfterViewInit {
  @ViewChild('unityCanvas', { static: true })
  canvas!: ElementRef<HTMLCanvasElement>;

  unityInstance: any;

  @HostListener('window:resize')
  onResize() {
    this.resizeCanvas();
  }

  ngAfterViewInit(): void {
    this.loadUnity();
  }

  private loadUnity(): void {
    const buildUrl = 'unity/Build';
    const loaderUrl = `${buildUrl}/dist_GPU.loader.js`;

    const dpr = this.getCorectDevbicePixelRatio();

    const config = {
      dataUrl: `${buildUrl}/dist_GPU.data`,
      frameworkUrl: `${buildUrl}/dist_GPU.framework.js`,
      codeUrl: `${buildUrl}/dist_GPU.wasm`,
      streamingAssetsUrl: 'unity/StreamingAssets',
      companyName: 'DefaultCompany',
      productName: 'mid-route-finder',
      productVersion: '0.1.0',
      //
      devicePixelRatio: dpr,
    };

    this.resizeCanvas();

    const script = document.createElement('script');
    script.src = loaderUrl;
    script.onload = () => {
      createUnityInstance(this.canvas.nativeElement, config, (progress) => {
        console.log(`Loading: ${Math.round(progress * 100)}%`);
      })
        .then((unityInstance) => {
          this.unityInstance = unityInstance;
          console.log('unityInstance: ', unityInstance);

          // Start a loop to keep canvas resolution synced in case of dynamic CSS changes
          const resizeLoop = () => {
            this.resizeCanvas();
            requestAnimationFrame(resizeLoop);
          };
          requestAnimationFrame(resizeLoop);
        })
        .catch((err) => {
          console.error(err);
        });
    };

    document.body.appendChild(script);
  }

  private resizeCanvas() {
    if (!this.canvas) return;

    const canvasEl = this.canvas.nativeElement;
    const dpr = this.getCorectDevbicePixelRatio();

    const displayWidth = Math.floor(canvasEl.clientWidth * dpr);
    const displayHeight = Math.floor(canvasEl.clientHeight * dpr);

    // Only resize if needed
    if (canvasEl.width !== displayWidth || canvasEl.height !== displayHeight) {
      canvasEl.width = displayWidth;
      canvasEl.height = displayHeight;
    }
  }

  getCorectDevbicePixelRatio(): number {
    return Math.min(window.devicePixelRatio * 2, 2.5);
  }
}
