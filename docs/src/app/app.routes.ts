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
    path: '**',
    redirectTo: '/docs/home'
  }
];