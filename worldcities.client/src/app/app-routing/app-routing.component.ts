import { NgModule } from '@angular/core';
import { RouterModule, Routes } from "@angular/router";
import { HomeComponent } from "../home/home.component";
import { CitiesComponent } from "../cities/cities.component"
import { CountriesComponent } from '../countries/countries.component';
import { CityEditComponent } from '../city-edit/city-edit.component';

const routes: Routes =
  [{ path: '', component: HomeComponent, pathMatch: 'full' },
    { path: 'cities', component: CitiesComponent },
    { path: 'countries', component: CountriesComponent },
    { path: 'city/:id', component: CityEditComponent },
    { path: 'city', component: CityEditComponent }];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule{

}
