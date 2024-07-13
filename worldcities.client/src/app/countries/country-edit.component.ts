import { Component, OnInit } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Country } from './country';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { environment } from '../../environments/environment.development';
import { Observable, map } from 'rxjs';

@Component({
  selector: 'app-country-edit',
  templateUrl: './country-edit.component.html',
  styleUrls: ['./country-edit.component.scss']
})
export class CountryEditComponent implements OnInit {

  title?: string;

  form!: FormGroup;

  country?: Country;

  id?: number;

  countries?: Country[];

  constructor(private fb: FormBuilder, private http: HttpClient,
   private router: Router, private activatedRoute: ActivatedRoute) {}

  ngOnInit() {
    this.form = this.fb.group({
      name: ['', Validators.required, this.isDupeField("name")],
      iso2: ['', [Validators.required, Validators.pattern(/^[a-zA-Z]{2}$/)], this.isDupeField("iso2")],
      iso3: ['', [Validators.required, Validators.pattern(/^[a-zA-Z]{3}$/)], this.isDupeField("iso3")],
    });

    this.loadData();
  }

  loadData() {
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    if (this.id) { //edit mode
      var url = environment.baseUrl + "api/Country/" + this.id;
      this.http.get<Country>(url).subscribe({
        next: (result) => {
          this.country = result;
          this.title = "Edit - " + this.country.name;
          this.form.patchValue(this.country);
        },
        error: (error) => {
          console.error(error);
        }
      });
    }
    else { //create mode
      this.title = "Create a new Country";
    }
  }

  onSubmit() {
    var country = (this.id) ? this.country : <Country>{};
    if (country) {
      country.name = this.form.controls['name'].value;
      country.iso2 = this.form.controls['iso2'].value;
      country.iso3 = this.form.controls['iso3'].value;

      if (this.id) {
        // edit
        var url = environment.baseUrl + 'api/Country/' + country.id;
        this.http
          .put<Country>(url, country)
          .subscribe({
            next: (result) => {
              console.log("Country " + country!.id + " has been updated.");
              this.router.navigate(['/countries']);
            },
            error: (error) => console.error(error)
          });
      }
      else {
        // create
        var url = environment.baseUrl + 'api/Country';
        this.http
          .post<Country>(url, country)
          .subscribe({
            next: (result) => {
              console.log("Country " + result.id + " has been created.");

              this.router.navigate(['/countries']);
            },
            error: (error) => console.error(error)
          });
      }
    }
  }

  isDupeField(fieldName: string): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{
      [key: string]: any
    } | null> => {
      var params = new HttpParams()
        .set("countryId", (this.id) ? this.id.toString() : "0")
        .set("fieldName", fieldName)
        .set("fieldValue", control.value);
      var url = environment.baseUrl + 'api/Country/IsDupeField';
      return this.http.post<boolean>(url, null, { params })
        .pipe(map(result => {
          return (result ? { isDupeField: true } : null);
        }));
    }
  }
}