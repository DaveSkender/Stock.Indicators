import { TestBed } from '@angular/core/testing';
import { MarkdownService } from './markdown.service';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

describe('MarkdownService', () => {
  let service: MarkdownService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(MarkdownService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should parse markdown with frontmatter', () => {
    const markdown = `---
title: Test Title
description: Test Description
---

# Test Content

This is a test.`;

    service.loadMarkdown('test').subscribe(result => {
      expect(result.frontmatter.title).toBe('Test Title');
      expect(result.frontmatter.description).toBe('Test Description');
      expect(result.content).toContain('<h1>Test Content</h1>');
    });

    const req = httpMock.expectOne('/docs/test.md');
    expect(req.request.method).toBe('GET');
    req.flush(markdown);
  });
});