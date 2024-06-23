import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup} from '@angular/forms';
import { City } from '../cities/city';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { Country } from '../countries/country'
import { count } from 'rxjs';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss']
})
export class CityEditComponent implements OnInit{
  //the view title
  title?: string;

  form!: FormGroup;

  city?: City;

  id?: number; //NULL when adding new one

  countris?: Country[];
  constructor(private activatedRoute: ActivatedRoute, private router: Router, private http: HttpClient) {

  }

  ngOnInit(): void {
    this.form = new FormGroup({
      name: new FormControl(''),
      lat: new FormControl(''),
      lon: new FormControl(''),
      countryId: new FormControl('')
    });
    this.loadData();
  }

  loadData() {
    this.loadCountries();

    var idParams = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParams ? +idParams : 0;
    if (this.id) {
      var url = environment.baseUrl + "api/City/" + this.id;
      this.http.get<City>(url).subscribe({
        next: (result) => {
          this.city = result;
          this.title = "Edit - " + this.city.name;
        },
        error: (error) => {
          console.error(error);
        }
      })
    }
    else {
      this.title = "Create a new city"
    }
  }

  loadCountries() {
    var url = environment.baseUrl + "api/Country/"
    var params = new HttpParams()
      .set("pageIndex", 0)
      .set("pageSize", "9999")
      .set("sortColumn", "name");
    this.http.get<any>(url, { params }).subscribe({
      next: (result) => {
        this.countris = result.data;
      },
      error: (error) => {
        console.error(error);
      }
    })
  }

  onSubmit() {
    var city = this.city;
    if (city) {
      city.name = this.form.controls['name'].value;
      city.lon = +this.form.controls['lon'].value;
      city.lat = +this.form.controls['lat'].value;
      city.countryId = +this.form.controls["countryId"].value;
      if (this.city) {
        var url = environment.baseUrl + "api/City/" + city.id;
        this.http.put<City>(url, city).subscribe({
          next: (result) => {
            console.log("City " + city!.id + "has been updated");
            this.router.navigate(['/cities']);
          },
          error: (error) => {
            console.error(error);
          }
        })
      }
      else {
        var url = environment.baseUrl + "api/City";
        this.http.post<City>(url, city).subscribe({
          next: (result) => {
            console.log("City" + result.id + "has been added");
            this.router.navigate(['/cities']);
          },
          error: (error) => {
            console.error(error);
          }
        })
      }
      
    }
  }
}
