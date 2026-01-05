import { MenuItem } from 'primeng/api';

export interface IMenuItem extends MenuItem {
    authorities?: string[];
    items?: IMenuItem[];
    onClick?: string;
    condition?: string;
    outletRouterLink?: string;
    // -------------------------    from MenuItem
    // label?: string;
    // routerLink?: string;
    // styleClass?: string;
    // icon?: string;
    // visible?: boolean;
}
