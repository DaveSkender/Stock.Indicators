import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { marked } from 'marked';

export interface MarkdownFrontmatter {
  title?: string;
  description?: string;
  keywords?: string;
  permalink?: string;
  [key: string]: string | undefined;
}

export interface MarkdownContent {
  content: string;
  frontmatter: MarkdownFrontmatter;
}

@Injectable({
  providedIn: 'root'
})
export class MarkdownService {
  private http = inject(HttpClient);

  loadMarkdown(slug: string): Observable<MarkdownContent> {
    return this.http.get(`/docs/${slug}.md`, { responseType: 'text' })
      .pipe(
        map(markdown => this.parseMarkdown(markdown))
      );
  }

  loadIndicatorMarkdown(indicator: string): Observable<MarkdownContent> {
    return this.http.get(`/_indicators/${indicator}.md`, { responseType: 'text' })
      .pipe(
        map(markdown => this.parseMarkdown(markdown))
      );
  }

  private parseMarkdown(markdown: string): MarkdownContent {
    const parts = markdown.split('---');
    
    if (parts.length < 3) {
      // No frontmatter
      return {
        content: marked.parse(markdown) as string,
        frontmatter: {}
      };
    }

    const frontmatterText = parts[1];
    const contentText = parts.slice(2).join('---');
    
    // Parse YAML frontmatter
    const frontmatter: MarkdownFrontmatter = {};
    frontmatterText.split('\n').forEach(line => {
      const colonIndex = line.indexOf(':');
      if (colonIndex > 0) {
        const key = line.substring(0, colonIndex).trim();
        const value = line.substring(colonIndex + 1).trim().replace(/^["']|["']$/g, '');
        frontmatter[key] = value;
      }
    });

    return {
      content: marked.parse(contentText.trim()) as string,
      frontmatter
    };
  }
}