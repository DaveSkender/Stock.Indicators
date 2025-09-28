import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface IndicatorCategory {
  name: string;
  type: string;
  subcategories?: IndicatorCategory[];
}

export interface IndicatorAlias {
  title: string;
  permalink: string;
  type: string;
}

export interface IndicatorInfo {
  title: string;
  path: string;
  type: string;
}

@Injectable({
  providedIn: 'root'
})
export class IndicatorDataService {
  private http = inject(HttpClient);

  getCategories(): Observable<IndicatorCategory[]> {
    return this.http.get<IndicatorCategory[]>('/assets/data/categories.json');
  }

  getAliases(): Observable<IndicatorAlias[]> {
    return this.http.get<IndicatorAlias[]>('/assets/data/aliases.json');
  }

  getIndicatorsList(): Observable<IndicatorInfo[]> {
    return this.http.get<string>('/assets/data/indicators-list.json', { responseType: 'text' as 'json' })
      .pipe(
        map(text => {
          // Process the indicators list from the _indicators folder
          return JSON.parse(text);
        })
      );
  }
}