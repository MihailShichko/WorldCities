import { Injectable } from '@angular/core';
import { City } from './city';
import { ApiResult, BaseService } from '../base.service';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { Country } from '../countries/country';



@Injectable({
  providedIn: 'root',
})
export class CityService extends BaseService<City>{
  constructor(http: HttpClient) {
    super(http);
  }

  override getData(pageIndex: number, pageSize: number, sortColumn: string, sortOrder: string,
    filterColumn: string | null, filterQuery: string | null): Observable<ApiResult<City>> {
    var url = this.getUrl("api/City");
    var params = new HttpParams()
      .set("pageIndex", pageIndex.toString())
      .set("pageSize", pageSize.toString())
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder);

    if (filterColumn && filterQuery) {
      params = params
        .set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery);
    }

    return this.http.get<ApiResult<City>>(url, { params });
  }

  override put(item: City): Observable<City> {
    var url = this.getUrl("api/City/" + item.id);
    return this.http.put<City>(url, item);
  }

  override post(item: City): Observable<City> {
    var url = this.getUrl("api/City");
    return this.http.post<City>(url, item);
  }
  override get(id: number): Observable<City> {
    var url = this.getUrl("api/City/" + id);
    return this.http.get<City>(url);
  }

  getCountries(pageIndex: number, pageSize: number, sortColumn: string,
    sortOrder: string, filterColumn: string | null, filterQuery: string | null): Observable<ApiResult<Country>> {
    var url = this.getUrl("api/Country");
    var params = new HttpParams()
      .set("pageIndex", pageIndex.toString())
      .set("pageSize", pageSize.toString())
      .set("sortColumn", pageSize)
      .set("sortOrder", sortOrder)      

    if (filterColumn && filterQuery) {
      params.set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery);
    }

    return this.http.get<ApiResult<Country>>(url, { params });
  }

  isDupeCity(item: City): Observable<boolean> {
    var url = this.getUrl("api/City/IsDupeCity");
    return this.http.post<boolean>(url, item);
  }
}
