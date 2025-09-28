import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./docs/docs.component').then(m => m.DocsComponent)
  },
  {
    path: 'guide',
    loadComponent: () => import('./docs/docs.component').then(m => m.DocsComponent)
  },
  {
    path: 'contributing',
    loadComponent: () => import('./docs/docs.component').then(m => m.DocsComponent)
  },
  {
    path: 'utilities',
    loadComponent: () => import('./docs/docs.component').then(m => m.DocsComponent)
  },
  {
    path: 'performance',
    loadComponent: () => import('./docs/docs.component').then(m => m.DocsComponent)
  },
  {
    path: 'indicators',
    loadComponent: () => import('./docs/docs.component').then(m => m.DocsComponent)
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