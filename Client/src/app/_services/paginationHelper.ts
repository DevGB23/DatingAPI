import { HttpClient, HttpParams } from "@angular/common/http";
import { PaginatedResults } from "../_models/Pagination";
import { map } from "rxjs";

export function getPaginatedResults<T> (url: string, params: HttpParams, http: HttpClient) {
    const paginatedResults: PaginatedResults<T> = new PaginatedResults<T>();
    
    return http.get<T>(url, { observe: 'response', params })
      .pipe(
        map(response => {
          if (response.body) {
            paginatedResults.result = response.body;
          }
          const pagination = response.headers.get('Pagination');

          if (pagination) {
            paginatedResults.pagination = JSON.parse(pagination);
          }

          return paginatedResults;
        })
      );
  }


  export function getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append('pageNumber', pageSize);
    params = params.append('pageSize', pageNumber);

    return params;
    
  }