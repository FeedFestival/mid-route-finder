import { Type } from '@angular/core';
import { DialogService, DynamicDialogConfig } from 'primeng/dynamicdialog';

export const openDialogAndGetInstance = <T>(
    component: Type<T>,
    config: DynamicDialogConfig,
    dialogService: DialogService,
): T => {
    const dialogRef = dialogService.open(component, config);
    const ref = dialogService.dialogComponentRefMap.get(dialogRef);
    ref?.changeDetectorRef.detectChanges();
    return ref?.instance.componentRef?.instance as T;
};

export const getLastOpenedDialogInstance = <T>(dialogService: DialogService): T | null => {
    const entry = dialogService.dialogComponentRefMap.entries().next().value;

    if (!entry) return null;

    const [, componentRef] = entry;
    componentRef.changeDetectorRef.detectChanges();

    return componentRef?.instance.componentRef?.instance as T;
};
