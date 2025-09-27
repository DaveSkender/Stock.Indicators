import { Component, OnInit, ChangeDetectionStrategy, signal, computed, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Title, Meta } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { catchError, of } from 'rxjs';

import { MarkdownService, MarkdownContent } from '../services/markdown.service';

@Component({
  selector: 'app-docs',
  template: `
    <div class="docs-content">
      @if (loading()) {
        <div class="loading">Loading...</div>
      } @else if (error()) {
        <div class="error">
          <h1>Page Not Found</h1>
          <p>The documentation page you're looking for doesn't exist.</p>
          <p><a href="/docs/home">Return to home</a></p>
        </div>
      } @else {
        <div [innerHTML]="htmlContent()"></div>
      }
    </div>
  `,
  styles: [`
    .docs-content {
      max-width: 800px;
      margin: 0 auto;
      padding: 2rem 1rem;
    }
    
    .loading {
      text-align: center;
      padding: 2rem;
    }
    
    .error {
      text-align: center;
      padding: 2rem;
    }
    
    .error a {
      color: #0366d6;
      text-decoration: none;
    }
    
    .error a:hover {
      text-decoration: underline;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule]
})
export class DocsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private title = inject(Title);
  private meta = inject(Meta);
  private markdownService = inject(MarkdownService);

  loading = signal(true);
  error = signal(false);
  content = signal<MarkdownContent | null>(null);
  
  htmlContent = computed(() => this.content()?.content || '');
  
  ngOnInit() {
    this.route.params.subscribe(params => {
      // Check if we're on an indicator route
      const indicator = params['indicator'];
      const slug = params['slug'] || 'home';
      
      if (indicator) {
        this.loadIndicatorContent(indicator);
      } else {
        this.loadContent(slug);
      }
    });
  }

  private loadIndicatorContent(indicator: string) {
    this.loading.set(true);
    this.error.set(false);
    
    this.markdownService.loadIndicatorMarkdown(indicator)
      .pipe(
        catchError(err => {
          console.error('Error loading indicator markdown:', err);
          this.error.set(true);
          this.loading.set(false);
          return of(null);
        })
      )
      .subscribe(content => {
        if (content) {
          this.content.set(content);
          this.updateMetaTags(content);
        }
        this.loading.set(false);
      });
  }

  private loadContent(slug: string) {
    this.loading.set(true);
    this.error.set(false);
    
    this.markdownService.loadMarkdown(slug)
      .pipe(
        catchError(err => {
          console.error('Error loading markdown:', err);
          this.error.set(true);
          this.loading.set(false);
          return of(null);
        })
      )
      .subscribe(content => {
        if (content) {
          this.content.set(content);
          this.updateMetaTags(content);
        }
        this.loading.set(false);
      });
  }

  private updateMetaTags(content: MarkdownContent) {
    const frontmatter = content.frontmatter;
    
    if (frontmatter.title) {
      this.title.setTitle(`${frontmatter.title} - Stock Indicators for .NET`);
    }
    
    if (frontmatter.description) {
      this.meta.updateTag({ name: 'description', content: frontmatter.description });
    }
    
    if (frontmatter.keywords) {
      this.meta.updateTag({ name: 'keywords', content: frontmatter.keywords });
    }
  }
}