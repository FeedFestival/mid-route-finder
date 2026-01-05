import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './shared/components';
import { MAIN_MENU_ITEM, SUB_MENU_ITEM, SUB_MENU_ITEMS } from './app.constants';
import { HomeRoutingModule } from './home/home-routing.module';

const routes: Routes = [
    {
        path: '',
        redirectTo: MAIN_MENU_ITEM.HOME,
        pathMatch: 'full',
    },
    {
        path: MAIN_MENU_ITEM.ENTITIES,
        loadComponent: () => import('./entities').then(m => m.EntitiesComponent),
        children: [
            {
                path: SUB_MENU_ITEMS[SUB_MENU_ITEM.ENTITIES_DASHBOARD].url,
                outlet: 'entitiestabview',
                loadComponent: () => import('./entities').then(m => m.EntitiesDashboardComponent),
            },
            {
                path: SUB_MENU_ITEMS[SUB_MENU_ITEM.ITEM_DEF].url,
                outlet: 'entitiestabview',
                loadComponent: () => import('./entities').then(m => m.ItemComponent),
            },
            {
                path: SUB_MENU_ITEMS[SUB_MENU_ITEM.UNIT_DEF].url,
                outlet: 'entitiestabview',
                loadComponent: () => import('./entities').then(m => m.UnitComponent),
            },
        ],
    },
    {
        path: MAIN_MENU_ITEM.ENTITY_DEF,
        loadComponent: () => import('./components/entity-def').then(m => m.EntityDefComponent),
        children: [
            {
                path: SUB_MENU_ITEMS[SUB_MENU_ITEM.DASHBOARD_ENTITY].url,
                outlet: 'entitydeftabview',
                loadComponent: () =>
                    import('./components/entity-def/dashboard-entity/dashboard-entity.component').then(
                        m => m.DashboardEntityComponent,
                    ),
            },
            {
                path: SUB_MENU_ITEMS[SUB_MENU_ITEM.UNIT_ENTITY].url,
                outlet: 'entitydeftabview',
                loadComponent: () =>
                    import('./components/entity-def/unit-entity/unit-entity.component').then(
                        m => m.UnitEntityComponent,
                    ),
            },
            {
                path: SUB_MENU_ITEMS[SUB_MENU_ITEM.INTERACTABLE_ENTITY].url,
                outlet: 'entitydeftabview',
                loadComponent: () =>
                    import('./components/entity-def/interactable-entity/interactable-entity.component').then(
                        m => m.InteractableEntityComponent,
                    ),
            },
            {
                path: SUB_MENU_ITEMS[SUB_MENU_ITEM.ITEM_ENTITY].url,
                outlet: 'entitydeftabview',
                loadComponent: () =>
                    import('./components/entity-def/item-entity/item-entity.component').then(
                        m => m.ItemEntityComponent,
                    ),
            },
            {
                path: SUB_MENU_ITEMS[SUB_MENU_ITEM.GENERAL_ENTITY].url,
                outlet: 'entitydeftabview',
                loadComponent: () =>
                    import('./components/entity-def/general-entity/general-entity.component').then(
                        m => m.GeneralEntityComponent,
                    ),
            },
        ],
    },
    {
        path: MAIN_MENU_ITEM.STORY,
        loadComponent: () => import('./components/story').then(m => m.StoryComponent),
        children: [
            {
                path: SUB_MENU_ITEMS[SUB_MENU_ITEM.DASHBOARD_STORY].url,
                outlet: 'storytabview',
                loadComponent: () => import('./components/story').then(m => m.DashboardStoryComponent),
            },
            {
                path: SUB_MENU_ITEMS[SUB_MENU_ITEM.PLAYER_INVENTORY].url,
                outlet: 'storytabview',
                loadComponent: () => import('./components/story').then(m => m.PlayerInventoryComponent),
            },
        ],
    },
    {
        path: '**',
        component: PageNotFoundComponent,
    },
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { enableTracing: false }), HomeRoutingModule],
    exports: [RouterModule],
})
export class AppRoutingModule {}
