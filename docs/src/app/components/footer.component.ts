import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <footer class="site-footer" *ngIf="showFooter">
      <div class="discussion-section">
        <p>
          <strong>Discuss this indicator</strong> 💬
        </p>
        <p>
          <a [href]="discussionUrl" target="_blank" rel="noopener">
            Join the community discussion about {{ indicatorName }}
          </a>
        </p>
      </div>
      
      <div class="back-to-top">
        <button (click)="scrollToTop()" class="back-to-top-btn">
          ↑ Back to top
        </button>
      </div>
    </footer>
  `,
  styles: [`
    .site-footer {
      margin-top: 3rem;
      padding: 2rem 0;
      border-top: 1px solid #e0e0e0;
      background-color: #fafafa;
    }
    
    .discussion-section {
      text-align: center;
      margin-bottom: 1.5rem;
    }
    
    .discussion-section p {
      margin: 0.5rem 0;
    }
    
    .discussion-section a {
      color: #0366d6;
      text-decoration: none;
    }
    
    .discussion-section a:hover {
      text-decoration: underline;
    }
    
    .back-to-top {
      text-align: center;
    }
    
    .back-to-top-btn {
      background: none;
      border: 1px solid #d1d5da;
      border-radius: 6px;
      color: #586069;
      cursor: pointer;
      font-size: 14px;
      padding: 8px 16px;
      text-decoration: none;
    }
    
    .back-to-top-btn:hover {
      background-color: #f6f8fa;
      border-color: #d1d5da;
      text-decoration: none;
    }
  `]
})
export class FooterComponent {
  @Input() indicatorName: string = '';
  @Input() discussionUrl: string = '';
  @Input() showFooter: boolean = false;

  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}