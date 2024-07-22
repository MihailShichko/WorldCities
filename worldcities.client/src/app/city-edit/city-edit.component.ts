import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { City } from '../cities/city';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { Country } from '../countries/country'
import { count, Observable, Subscription } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { BaseFormComponent } from '../base-form.component';
import { CityService } from '../cities/city.servise';


@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss']
})
export class CityEditComponent extends BaseFormComponent implements OnInit{
  //the view title
  title?: string;

  city?: City;

  id?: number; //NULL when adding new one

  private subscriptions: Subscription = new Subscription();

  countries?: Country[];
  constructor(private activatedRoute: ActivatedRoute, private router: Router, private http: HttpClient,
    private cityService: CityService) {
    super();
  }

  ngOnInit(): void {
    this.form = new FormGroup({
      name: new FormControl('', Validators.required),
      lat: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)
      ]),
      lon: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)
      ]),
      countryId: new FormControl('', Validators.required)
    }, null, this.isDupeCity());

    this.subscriptions.add(this.form.valueChanges.subscribe(() => {
      if (!this.form.dirty) {
        this.log("Form Model has been loaded");
      }
      else {
        this.log("Form has been updated");
      }
    }));

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
      return this.cityService.isDupeCity(city).pipe(map(result => { 

        return (result ? { isDupeCity: true } : null);
      }));
    }
  }

  loadData() {
    this.loadCountries();
    var idParams = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParams ? +idParams : 0;
    if (this.id)//EDIT MODE
    {
      this.cityService.get(this.id).subscribe({
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
    this.cityService.getCountries(0,
      999,
      "name",
      "asc",
      null,
      null).subscribe({
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
        this.cityService.put(city).subscribe({
          next: (result) => {
            console.log("City " + city!.id + " has been updated.");

            this.router.navigate(['/cities']);
          },
          error: (error) => console.error(error)
        });
      }
      else //add
      {
        this.cityService.post(city).subscribe({
          next: (result) => {

            console.log("City " + result.id + " has been created.");

            this.router.navigate(['/cities']);
          },
          error: (error) => console.error(error)
        });
      }
    }
  }

  log(str: string) {
    console.log("["
      + new Date().toLocaleString()
      + "] " + str);
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }
}
