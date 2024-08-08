import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResult, BaseService } from '../base.service';
import { Country, CountryBack } from './country';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CountryService extends BaseService<Country> {
  override getData(pageIndex: number, pageSize: number, sortColumn: string, sortOrder: string,
    filterColumn: string | null, filterQuery: string | null): Observable<ApiResult<Country>> {
    var url = this.getUrl("api/Country");
    var params = new HttpParams()
      .set("pageIndex", pageIndex.toString())
      .set("pageSize", pageSize.toString())
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder);

    if (filterColumn && filterQuery)
      params.set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery);

    return this.http.get<ApiResult<Country>>(url, { params }); 
  }
  override put(item: Country): Observable<Country> {
    var url = this.getUrl("api/Country/" + item.id);
    return this.http.put<Country>(url, item);
  }
  override post(item: Country): Observable<Country> {
    var url = this.getUrl("api/Country");
    return this.http.post<Country>(url, item);
  }
  override get(id: number): Observable<Country> {
    var url = this.getUrl("api/Country/" + id);
    return this.http.get<Country>(url);
  }

  isDupeField(countryId: number, fieldName: string, fieldValue: string): Observable<boolean> {
    var url = this.getUrl("api/Country/IsDupeField")
    var params = new HttpParams()
      .set("countryId", countryId)
      .set("fieldName", fieldName)
      .set("fieldValue", fieldValue);
    return this.http.post<boolean>(url, null, { params });
  }

  constructor(http: HttpClient) {
    super(http);
  }
}
