import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ScullyLibModule } from '@scullyio/ng-lib';

import { DocsRoutingModule } from './docs-routing-module';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    DocsRoutingModule,
    ScullyLibModule
  ]
})
export class DocsModule { }
