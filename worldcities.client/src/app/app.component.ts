import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth/auth-service.service';

import { ConnectionService, ConnectionServiceOptions, ConnectionState } from
    'ng-connection-service';
import { Observable, map } from 'rxjs';
import { environment } from './../environments/environment.development';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent implements OnInit{
  title = 'WorldCities';
    //public isOffline: Observable<boolean>;

    constructor(private authService: AuthService, private connectionService: ConnectionService) {
        //const options: ConnectionServiceOptions = {
        //    enableHeartbeat: true,
        //    heartbeatUrl: environment.baseUrl + 'api/heartbeat',
        //    heartbeatInterval: 10000
        //};
        //this.isOffline = this.connectionService.monitor(options)
        //    .pipe(map(state => !state.hasNetworkConnection || state.
        //        hasInternetAccess));
    }

  ngOnInit(): void {
    this.authService.init();
  }

} 
