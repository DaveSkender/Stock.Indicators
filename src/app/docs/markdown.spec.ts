import { TestBed } from '@angular/core/testing';

import { Markdown } from './markdown';

describe('Markdown', () => {
  let service: Markdown;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Markdown);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
