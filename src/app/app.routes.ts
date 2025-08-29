import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'docs',
    loadChildren: () => import('./docs/docs-module').then(m => m.DocsModule)
  },
  {
    path: '',
    redirectTo: '/docs/index',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: '/docs/index'
  }
];
