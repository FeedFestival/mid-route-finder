import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'pad0',
    standalone: true,
})
export class Pad0Pipe implements PipeTransform {
    transform(value: number, width: number = 2): string {
        return value.toString().padStart(width, '0');
    }
}
