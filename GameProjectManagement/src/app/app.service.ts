import { Injectable } from '@angular/core';
import { WriteFileOptions } from 'fs';
import { from, Observable, Subject } from 'rxjs';
import { ElectronService } from './core/services';

export const SUCCESS = 'Success';

@Injectable({ providedIn: 'root' })
export class AppService {
    constructor(private readonly electronService: ElectronService) {
        if (electronService.isElectron) {
            console.log(process.env);
            console.log('Run in electron');
            console.log('Electron ipcRenderer', this.electronService.ipcRenderer);
            console.log('NodeJS childProcess', this.electronService.childProcess);
        } else {
            console.log('Run in browser');
        }
    }

    readFile(absolutePath: string, file: string, ext: string = 'json'): Observable<any> {
        const completePath = `${absolutePath}/${file}.${ext}`;
        const sub = new Subject();

        this.electronService.fs.readFile(completePath, 'utf8', (error, data) => {
            if (!data) {
                sub.next(undefined);
                return;
            }

            let result;
            if (ext === 'json')
                result = JSON.parse(data);
            
            sub.next(result);
        });

        return sub.asObservable();
    }

    saveFile(data: string, absolutePath: string, file: string, ext: string = 'json'): Observable<string> {
        const completePath = `${absolutePath}/${file}.${ext}`;
        const sub = new Subject<string>();

        const options: WriteFileOptions = {
            encoding: 'utf8',
        };

        this.electronService.fs.writeFile(completePath, data, options, data => {
            // const json = JSON.parse(data);
            sub.next(`${SUCCESS}: ${data}`);
        });

        return sub.asObservable();
    }

    getAbsolutePath(projectPath: string): string {
        const mainDir = this.getMainDirectory();
        return `${mainDir}/${projectPath}`;
    }

    openFolderRelativeLocation(path: string): void {
        const absolutePath = this.getAbsolutePath(path);
        this.electronService.childProcess.exec(`start "" "${absolutePath}"`);
    }

    openDialog(options: Electron.OpenDialogOptions): Observable<string[]> {
        return from(
            this.electronService.openFileOrFolderDialog(options).then(result => {
                if (!result.canceled) {
                    return result.filePaths;
                }
                return [];
            }),
        );
    }

    getMainDirectory(): string {
        const currentDir = __dirname;
        const mainDir = currentDir.split('story-manager')[0];
        const parentDir = this.electronService.path.dirname(mainDir);
        return parentDir;
    }

    writeDirsIfNotExists(dirPath: string): void {
        const pathRef = this.electronService.path;
        const fsRef = this.electronService.fs;
        const segments = pathRef.normalize(dirPath).split(pathRef.sep);
        let currentPath = this.getMainDirectory();

        for (const segment of segments) {
            if (!segment) continue; // Skip empty segments from absolute paths
            currentPath = currentPath ? pathRef.join(currentPath, segment) : segment;

            if (!fsRef.existsSync(currentPath)) {
                fsRef.mkdirSync(currentPath);
                console.log('Created folder:', currentPath);
            }
        }
    }
}
