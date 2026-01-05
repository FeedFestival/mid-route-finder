export interface Column {
    field: string;
    header: string;
    filterMatchMode?: 'contains';
    noFilter?: boolean;
    width?: string;
}
