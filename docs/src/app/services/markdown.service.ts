import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
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
    const normalizedSlug = this.normalizeSlug(slug);
    return this.http.get(this.buildPagePath(normalizedSlug), { responseType: 'text' })
      .pipe(
        map(markdown => this.parseMarkdown(markdown)),
        catchError((error: HttpErrorResponse) => {
          if (error.status === 404) {
            return this.loadFromLegacyLocation(normalizedSlug);
          }
          return throwError(() => error);
        })
      );
  }

  private loadFromLegacyLocation(slug: string): Observable<MarkdownContent> {
    return this.http.get(this.buildLegacyPath(slug), { responseType: 'text' })
      .pipe(
        map(markdown => this.parseMarkdown(markdown)),
        switchMap(parsed => {
          const relativePath = parsed.frontmatter?.['relative_path'];
          if (relativePath) {
            const actualPath = this.normalizeRelativePath(relativePath);
            return this.http.get(actualPath, { responseType: 'text' })
              .pipe(map(markdown => this.parseMarkdown(markdown)));
          }
          return of(parsed);
        })
      );
  }

  private normalizeSlug(slug: string): string {
  const trimmed = slug.replace(/^\/+/g, '').replace(/\/+$/g, '');
  return trimmed.length > 0 ? trimmed : 'home';
  }

  private buildPagePath(slug: string): string {
    return `/pages/${slug}.md`;
  }

  private buildLegacyPath(slug: string): string {
    return `/${slug}.md`;
  }

  private normalizeRelativePath(relativePath: string): string {
    return relativePath.startsWith('/') ? relativePath : `/${relativePath}`;
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
