import { Component, OnInit, ChangeDetectionStrategy, signal, computed, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Title, Meta } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { catchError, of } from 'rxjs';

import { MarkdownService, MarkdownContent } from '../services/markdown.service';
import { FooterComponent } from '../components/footer.component';

@Component({
  selector: 'app-docs',
  standalone: true,
  templateUrl: './docs.component.html',
  styleUrls: ['./docs.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FooterComponent]
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
  indicatorName = signal<string>('');
  discussionUrl = signal<string>('');
  currentIndicator = signal<string>('');
  
  ngOnInit() {
    this.route.params.subscribe(params => {
      // Check if we're on an indicator route
      const indicator = params['indicator'];
      
      if (indicator) {
        this.currentIndicator.set(indicator);
        this.indicatorName.set(indicator);
        this.discussionUrl.set(`https://github.com/DaveSkender/Stock.Indicators/discussions`);
        this.loadIndicatorContent(indicator);
      } else {
        this.currentIndicator.set('');
        // Determine which page to load based on current route
        const currentPath = this.router.url.split('?')[0]; // Remove query params
        let slug: string;
        
        if (currentPath === '/' || currentPath === '') {
          slug = 'home'; // Load home.md for root route
        } else {
          slug = currentPath.startsWith('/') ? currentPath.substring(1) : currentPath;
        }
        
        this.loadContent(slug);
      }
    });
  }

  isIndicatorPage(): boolean {
    return !!this.currentIndicator();
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
