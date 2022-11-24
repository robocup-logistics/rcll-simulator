import { Injectable } from '@angular/core';
import {Zone} from "../Interfaces/Zone";
import {Robot} from "../Interfaces/Robot";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {Machine} from "../Interfaces/Machine";

@Injectable({
  providedIn: 'root'
})
export class ZonesService {
  private Client: HttpClient;
  private robots : Zone[] | undefined;
  private BaseUrl : string;
  constructor(private http: HttpClient) {
    this.Client = http;
    this.BaseUrl = "http://localhost:8000/";

  }

  getZones(): Observable<Zone[]> {
    //console.log(new Date() + ": Get Zones");
    return this.Client.get<Zone[]>(this.BaseUrl + "zones")
  };
  getRobots(): Observable<Robot[]>{
    //console.log(new Date() + ": Get Zones");
    return this.Client.get<Robot[]>(this.BaseUrl + "robots")
  }
  getMachines(): Observable<Machine[]>{
    //console.log(new Date() + ": Get Zones");
    return this.Client.get<Machine[]>(this.BaseUrl + "machines")
  }
}
