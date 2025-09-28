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

# Test Content

This is a test.`;

    service.loadMarkdown('test').subscribe(result => {
      expect(result.frontmatter.title).toBe('Test Title');
      expect(result.frontmatter.description).toBe('Test Description');
      expect(result.content).toContain('<h1>Test Content</h1>');
    });

    const req = httpMock.expectOne('/pages/test.md');
    expect(req.request.method).toBe('GET');
    req.flush(markdown);
  });

  it('should load home page when slug is empty', () => {
    const markdown = `---
title: Home
description: Home page

# Welcome

Intro.`;

    service.loadMarkdown('').subscribe(result => {
      expect(result.frontmatter.title).toBe('Home');
    });

    const req = httpMock.expectOne('/pages/home.md');
    expect(req.request.method).toBe('GET');
    req.flush(markdown);
  });

  it('should fallback to legacy markdown location with relative path', () => {
    const legacyStub = `---
title: Legacy Stub
relative_path: pages/actual.md
---

<!-- moved -->`;

    const actualMarkdown = `---
title: Actual Title
description: Actual Description
---

# Real Content`;

    service.loadMarkdown('legacy').subscribe(result => {
      expect(result.frontmatter.title).toBe('Actual Title');
      expect(result.frontmatter.description).toBe('Actual Description');
      expect(result.content).toContain('<h1>Real Content</h1>');
    });

    const modernRequest = httpMock.expectOne('/pages/legacy.md');
    modernRequest.flush('Not Found', { status: 404, statusText: 'Not Found' });

    const legacyRequest = httpMock.expectOne('/legacy.md');
    legacyRequest.flush(legacyStub);

    const actualRequest = httpMock.expectOne('/pages/actual.md');
    actualRequest.flush(actualMarkdown);
  });
});
