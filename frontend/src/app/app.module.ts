import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { HttpClientXsrfModule } from '@angular/common/http';

import { HttpClientInMemoryWebApiModule } from 'angular-in-memory-web-api';
import { InMemoryDataService } from './in-memory-data.service';

import { RequestCache, RequestCacheWithMap } from './request-cache.service';

import { AppComponent } from './app.component';
import { AuthService } from './auth.service';

import { TestComponentComponent } from './test-component/test-component.component';
import { ZoneComponent } from './zone/zone.component';
import {KonvaModule} from "ng2-konva";

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    KonvaModule,
    // import HttpClientModule after BrowserModule.
    HttpClientModule,
    HttpClientXsrfModule.withOptions({
      cookieName: 'My-Xsrf-Cookie',
      headerName: 'My-Xsrf-Header',
    }),

    // The HttpClientInMemoryWebApiModule module intercepts HTTP requests
    // and returns simulated server responses.
    // Remove it when a real server is ready to receive requests.
    HttpClientInMemoryWebApiModule.forRoot(
      InMemoryDataService, {
        dataEncapsulation: false,
        passThruUnknownUrl: true,
        put204: false // return entity after PUT/update
      }
    ),
  ],
  declarations: [
    AppComponent,
    TestComponentComponent,
    ZoneComponent,
  ],
  providers: [
    AuthService,
    { provide: RequestCache, useClass: RequestCacheWithMap },
  ],
  bootstrap: [ AppComponent ]
})
export class AppModule {}
