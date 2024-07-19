import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { City } from './city';
import { environment } from '../../environments/environment.development';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent} from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CityService } from './city.servise';


@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.scss']
})
export class CitiesComponent implements OnInit {
  public displayedColumns: string[] = ["id", "name", "lat", "lon", "countryName"];

  public cities!: MatTableDataSource<City>;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  filterTextChanged: Subject<string> = new Subject<string>();

  defaultPageIndex:number = 0;

  defaultPageSize: number = 10;

  defaultFilterColumn: string = "name";

  filterQuery?: string | null;

  public defaultSortColumn: string = "name";
  public defaultSortOrder: "asc" | "desc" = "asc";

  constructor(private cityService: CityService) {

  }

  ngOnInit() {
    this.loadData(null);
  }

  onFilterTextChanged(filterText: string) {
    if (!this.filterTextChanged.observed) {
      this.filterTextChanged.pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe(query => {
          this.loadData(query);
        });
    }

    this.filterTextChanged.next(filterText);
  }

  loadData(query?:string | null) {
    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;
    this.filterQuery = query;
    this.getData(pageEvent);
  }

  getData(event: PageEvent) {
    var sortColumn = (this.sort)
      ? this.sort.active
      : this.defaultSortColumn;
    var sortOrder = (this.sort)
      ? this.sort.direction
      : this.defaultSortOrder;
    var filterColumn = (this.filterQuery)
      ? this.defaultFilterColumn
      : null;
    var filterQuery = (this.filterQuery)
      ? this.filterQuery
      : null;
    this.cityService.getData(
      event.pageIndex,
      event.pageSize,
      sortColumn,
      sortOrder,
      filterColumn,
      filterQuery).subscribe({
        next: (result) => {

        },
        error: (error) => {

        }
      })
   
  }

}
