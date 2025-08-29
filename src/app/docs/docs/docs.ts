import { Component, OnInit } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';
import { ScullyRoutesService, ScullyContentComponent } from '@scullyio/ng-lib';

@Component({
  selector: 'app-docs',
  standalone: true,
  imports: [ScullyContentComponent],
  templateUrl: './docs.html',
  styleUrl: './docs.scss'
})
export class Docs implements OnInit {

  constructor(
    private title: Title,
    private meta: Meta,
    private scully: ScullyRoutesService,
  ) {}

  ngOnInit() {
    this.scully.getCurrent().subscribe((route: any) => {
      if (route?.title) {
        this.title.setTitle(route.title);
      }
      if (route?.['description']) {
        this.meta.updateTag({ name: 'description', content: route['description'] });
      }
      if (route?.['keywords']) {
        this.meta.updateTag({ name: 'keywords', content: route['keywords'] });
      }
    });
  }
}
