import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Docs } from './docs/docs';

const routes: Routes = [
  {
    path: ':slug',
    component: Docs,
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DocsRoutingModule { }
