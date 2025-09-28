import { Component, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-not-found',
  standalone: true,
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NotFoundComponent implements OnInit {
  private titleService = inject(Title);
  
  ngOnInit() {
    this.titleService.setTitle('Page not found - Stock Indicators for .NET');
  }
}
