import { Component, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-not-found',
  template: `
    <div style="text-align:center; margin-top:50px;">
      <h1>Your strategy was unprofitable</h1>
      <p><strong>error 404 ~ page not found</strong></p>
      <p><a href="/" class="btn btn-primary">Return to Home</a></p>
    </div>
  `,
  styles: [`
    .btn {
      display: inline-block;
      padding: 0.5rem 1rem;
      margin-top: 1rem;
      background-color: #007bff;
      color: white;
      text-decoration: none;
      border-radius: 0.25rem;
    }
    .btn:hover {
      background-color: #0056b3;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NotFoundComponent implements OnInit {
  private titleService = inject(Title);
  
  ngOnInit() {
    this.titleService.setTitle('Page not found - Stock Indicators for .NET');
  }
}