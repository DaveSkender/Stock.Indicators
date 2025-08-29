import { Component, OnInit } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MarkdownService } from '../markdown';

@Component({
  selector: 'app-docs',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="docs-container">
      <div [innerHTML]="content"></div>
    </div>
  `,
  styleUrl: './docs.scss'
})
export class Docs implements OnInit {
  content = '';

  constructor(
    private title: Title,
    private meta: Meta,
    private route: ActivatedRoute,
    private markdownService: MarkdownService
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const slug = params['slug'] || 'index';
      this.loadContent(slug);
    });
  }

  private loadContent(slug: string) {
    this.markdownService.loadMarkdownFile(slug).subscribe({
      next: (docContent) => {
        this.content = docContent.content;
        
        if (docContent.frontmatter.title) {
          this.title.setTitle(docContent.frontmatter.title);
        }
        
        if (docContent.frontmatter.description) {
          this.meta.updateTag({ name: 'description', content: docContent.frontmatter.description });
        }
        
        if (docContent.frontmatter.keywords) {
          this.meta.updateTag({ name: 'keywords', content: docContent.frontmatter.keywords });
        }
      },
      error: (error) => {
        console.error('Error loading markdown file:', error);
        this.content = `<h1>Page Not Found</h1><p>The requested page "${slug}" could not be found.</p>`;
      }
    });
  }
}
