export const route = (...args: string[]) => {
    let url = '';
    args.forEach(a => (url += '/' + a));
    return url;
};

export const GAME_MAIN_DIR = 'Top Down';
export const ART_MAIN_DIR = 'Art Development';

export enum MAIN_MENU_ITEM {
    HOME = 'home',
    ENTITIES = 'entities',
    ENTITY_DEF = 'entity-def',
    STORY = 'story',
}

export const MENU_ITEMS: Record<MAIN_MENU_ITEM, any> = {
    [MAIN_MENU_ITEM.HOME]: {
        id: 0,
        label: MAIN_MENU_ITEM.HOME,
        url: '/' + MAIN_MENU_ITEM.HOME,
    },
    [MAIN_MENU_ITEM.ENTITIES]: {
        id: 1,
        label: MAIN_MENU_ITEM.ENTITIES,
        url: '/' + MAIN_MENU_ITEM.ENTITIES,
    },
    [MAIN_MENU_ITEM.ENTITY_DEF]: {
        id: 2,
        label: MAIN_MENU_ITEM.ENTITY_DEF,
        url: '/' + MAIN_MENU_ITEM.ENTITY_DEF,
    },
    [MAIN_MENU_ITEM.STORY]: {
        id: 3,
        label: MAIN_MENU_ITEM.STORY,
        url: '/' + MAIN_MENU_ITEM.STORY,
    },
};

export enum SUB_MENU_ITEM {
    // ENTITIES
    ENTITIES_DASHBOARD = 'entities-dashboard',
    ITEM_DEF = 'item-def',
    UNIT_DEF = 'unit-def',
    // ENTITY_DEF
    DASHBOARD_ENTITY = 'dashboard-entity',
    UNIT_ENTITY = 'unit-entity',
    INTERACTABLE_ENTITY = 'interactable-entity',
    ITEM_ENTITY = 'item-entity',
    GENERAL_ENTITY = 'general-entity',
    // STORY
    DASHBOARD_STORY = 'dashboard-story',
    PLAYER_INVENTORY = 'player-inventory',
}

export const SUB_MENU_ITEMS: Record<SUB_MENU_ITEM, any> = {
    // ENTITIES
    [SUB_MENU_ITEM.ENTITIES_DASHBOARD]: {
        id: 0,
        name: 'Dashboard',
        label: SUB_MENU_ITEM.ENTITIES_DASHBOARD,
        url: SUB_MENU_ITEM.ENTITIES_DASHBOARD,
        routeUrl: '/' + SUB_MENU_ITEM.ENTITIES_DASHBOARD,
    },
    [SUB_MENU_ITEM.ITEM_DEF]: {
        id: 1,
        name: 'Item',
        label: SUB_MENU_ITEM.ITEM_DEF,
        url: SUB_MENU_ITEM.ITEM_DEF,
        routeUrl: '/' + SUB_MENU_ITEM.ITEM_DEF,
    },
    [SUB_MENU_ITEM.UNIT_DEF]: {
        id: 2,
        name: 'Unit',
        label: SUB_MENU_ITEM.UNIT_DEF,
        url: SUB_MENU_ITEM.UNIT_DEF,
        routeUrl: '/' + SUB_MENU_ITEM.UNIT_DEF,
    },
    // ENTITY_DEF
    [SUB_MENU_ITEM.DASHBOARD_ENTITY]: {
        id: 0,
        name: 'Dashboard',
        label: SUB_MENU_ITEM.DASHBOARD_ENTITY,
        url: SUB_MENU_ITEM.DASHBOARD_ENTITY,
        routeUrl: '/' + SUB_MENU_ITEM.DASHBOARD_ENTITY,
    },
    [SUB_MENU_ITEM.UNIT_ENTITY]: {
        id: 1,
        name: 'Unit Entity',
        label: SUB_MENU_ITEM.UNIT_ENTITY,
        url: SUB_MENU_ITEM.UNIT_ENTITY,
        routeUrl: '/' + SUB_MENU_ITEM.UNIT_ENTITY,
    },
    [SUB_MENU_ITEM.INTERACTABLE_ENTITY]: {
        id: 2,
        name: 'Interactable Entity',
        label: SUB_MENU_ITEM.INTERACTABLE_ENTITY,
        url: SUB_MENU_ITEM.INTERACTABLE_ENTITY,
        routeUrl: '/' + SUB_MENU_ITEM.INTERACTABLE_ENTITY,
    },
    [SUB_MENU_ITEM.ITEM_ENTITY]: {
        id: 3,
        name: 'Item Entity',
        label: SUB_MENU_ITEM.ITEM_ENTITY,
        url: SUB_MENU_ITEM.ITEM_ENTITY,
        routeUrl: '/' + SUB_MENU_ITEM.ITEM_ENTITY,
    },
    [SUB_MENU_ITEM.GENERAL_ENTITY]: {
        id: 4,
        name: 'General Entity',
        label: SUB_MENU_ITEM.GENERAL_ENTITY,
        url: SUB_MENU_ITEM.GENERAL_ENTITY,
        routeUrl: '/' + SUB_MENU_ITEM.GENERAL_ENTITY,
    },
    // STORY
    [SUB_MENU_ITEM.DASHBOARD_STORY]: {
        id: 4,
        name: 'Dashboard Story',
        label: SUB_MENU_ITEM.DASHBOARD_STORY,
        url: SUB_MENU_ITEM.DASHBOARD_STORY,
        routeUrl: '/' + SUB_MENU_ITEM.DASHBOARD_STORY,
    },
    [SUB_MENU_ITEM.PLAYER_INVENTORY]: {
        id: 5,
        name: 'Inventory',
        label: SUB_MENU_ITEM.PLAYER_INVENTORY,
        url: SUB_MENU_ITEM.PLAYER_INVENTORY,
        routeUrl: '/' + SUB_MENU_ITEM.PLAYER_INVENTORY,
    },
    
};

export const INVENTORY_PARENT_ROUTE = route(
    MAIN_MENU_ITEM.ENTITIES,
    SUB_MENU_ITEMS[SUB_MENU_ITEM.ENTITIES_DASHBOARD].url,
);

export const ENTITY_PARENT_ROUTE = route(
    MAIN_MENU_ITEM.ENTITY_DEF,
    SUB_MENU_ITEMS[SUB_MENU_ITEM.DASHBOARD_ENTITY].url,
);

export const STORY_PARENT_ROUTE = route(
    MAIN_MENU_ITEM.STORY,
    SUB_MENU_ITEMS[SUB_MENU_ITEM.DASHBOARD_STORY].url,
);
