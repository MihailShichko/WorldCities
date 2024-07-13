import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { City } from '../cities/city';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { Country } from '../countries/country'
import { count, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

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

  countries?: Country[];
  constructor(private activatedRoute: ActivatedRoute, private router: Router, private http: HttpClient) {

  }

  ngOnInit(): void {
    this.form = new FormGroup({
      name: new FormControl('', Validators.required),
      lat: new FormControl('', Validators.required),
      lon: new FormControl('', Validators.required),
      countryId: new FormControl('', Validators.required)
    }, null, this.isDupeCity());

    this.loadData();
  }

  isDupeCity(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var city = <City>{};
      city.id = (this.id) ? this.id : 0;
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = +this.form.controls['countryId'].value;

      var url = environment.baseUrl + 'api/City/IsDupeCity'; 
      return this.http.post<boolean>(url, city).pipe(map(result => { 

        return (result ? { isDupeCity: true } : null);
      }));
    }
  }

  loadData() {
    console.log("before load counties")
    this.loadCountries();
    console.log("countries loaded")
    var idParams = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParams ? +idParams : 0;
    if (this.id)//EDIT MODE
    {
      var url = environment.baseUrl + "api/City/" + this.id;
      this.http.get<City>(url).subscribe({
        next: (result) => {
          this.city = result;
          this.title = "Edit - " + this.city.name;
          this.form.patchValue(this.city);
        },
        error: (error) => {
          console.error(error);
        }
      })
    }
    else //ADD MODE 
    {
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
        this.countries = result.data;
      },
      error: (error) => {
        console.error(error);
      }
    })
  }

  onSubmit(): void {
    console.log("OnSubmit");

    var city = (this.id) ? this.city : <City>{};
    if (city)
    {
      city.name = this.form.controls['name'].value;
      city.lon = +this.form.controls['lon'].value;
      city.lat = +this.form.controls['lat'].value;
      city.countryId = +this.form.controls["countryId"].value;
      if (this.id) //edit
      {
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
      else //add
      {
        var url = environment.baseUrl + "api/City";
        this.http.post<City>(url, city).subscribe({
          next: (result) => {
            console.log("City" + result.id + "has been created");
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
