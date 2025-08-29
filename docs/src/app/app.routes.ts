import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/docs/home',
    pathMatch: 'full'
  },
  {
    path: 'docs',
    loadComponent: () => import('./docs/docs.component').then(m => m.DocsComponent)
  },
  {
    path: 'docs/:slug',
    loadComponent: () => import('./docs/docs.component').then(m => m.DocsComponent)
  },
  {
    path: 'indicators',
    redirectTo: '/docs/indicators',
    pathMatch: 'full'
  },
  {
    path: 'indicators/:indicator',
    loadComponent: () => import('./docs/docs.component').then(m => m.DocsComponent)
  },
  {
    path: '404',
    loadComponent: () => import('./not-found/not-found.component').then(m => m.NotFoundComponent)
  },
  {
    path: '**',
    loadComponent: () => import('./not-found/not-found.component').then(m => m.NotFoundComponent)
  }
];