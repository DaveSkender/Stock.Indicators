import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { marked } from 'marked';

export interface DocContent {
  frontmatter: {
    title?: string;
    description?: string;
    keywords?: string;
    order?: number;
  };
  content: string;
}

@Injectable({
  providedIn: 'root'
})
export class MarkdownService {
  
  constructor(private http: HttpClient) {}

  loadMarkdownFile(filename: string): Observable<DocContent> {
    return this.http.get(`/docs/${filename}.md`, { responseType: 'text' }).pipe(
      map(markdown => this.parseMarkdown(markdown))
    );
  }

  private parseMarkdown(markdown: string): DocContent {
    // Parse frontmatter
    const frontmatterRegex = /^---\s*\n([\s\S]*?)\n---\s*\n([\s\S]*)$/;
    const match = markdown.match(frontmatterRegex);
    
    let frontmatter: any = {};
    let content = markdown;

    if (match) {
      // Parse YAML frontmatter (simple implementation)
      const frontmatterText = match[1];
      const markdownContent = match[2];

      frontmatterText.split('\n').forEach(line => {
        const colonIndex = line.indexOf(':');
        if (colonIndex > 0) {
          const key = line.substring(0, colonIndex).trim();
          const value = line.substring(colonIndex + 1).trim().replace(/^['"]|['"]$/g, '');
          frontmatter[key] = value;
        }
      });

      content = markdownContent;
    }

    // Convert markdown to HTML
    const htmlContent = marked(content);

    return {
      frontmatter,
      content: htmlContent as string
    };
  }
}
