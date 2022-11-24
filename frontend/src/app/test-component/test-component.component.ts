import {Component, Injectable, OnInit} from '@angular/core';
import {HttpClient, HttpHeaders, HttpRequest, HttpResponse} from "@angular/common/http";
import {Zone} from "../Interfaces/Zone";
import {Robot} from "../Interfaces/Robot";
import {Machine} from "../Interfaces/Machine"
import {KonvaModule} from "ng2-konva";
import {BehaviorSubject, interval, Observable, of, Subscription} from "rxjs";
import {ZonesService} from "../services/zones.service";
import Konva from "konva";
import Vector2d = Konva.Vector2d;
import {KonvaZone} from "./KonvaZone";
import {KonvaMachine} from "./KonvaMachine";
import {KonvaRobot} from "./KonvaRobot";

export enum TeamColor {
  CYAN = 0,
  MAGENTA = 1
}

export enum RingColors {
  RingBlue = 1,
  RingGreen = 2,
  RingOrange = 3,
  RingYellow = 4,
}

export enum BaseColor {
  BaseUncolored = 0,
  BaseRed = 1,
  BaseBlack = 2,
  BaseSilver = 3,
}

export enum CapColor {
  CapBlack = 1,
  CapGrey = 2,
}

@Component({
  selector: 'app-test-component',
  templateUrl: './test-component.component.html',
  styleUrls: ['./test-component.component.css']
})

@Injectable()
export class TestComponentComponent implements OnInit {

  private Client: HttpClient;
  public zonesData: Zone[] | undefined;
  public robotsData: Robot[] | undefined;
  public machinesData: Machine[] | undefined;

  public ZoneService: ZonesService;
  private subscription: Subscription;
  private initSubscription: Subscription;

  private stage: Konva.Stage | undefined;
  private layer: Konva.Layer | undefined;
  private ZonesMap: Map<number, KonvaZone>;
  private robotsCyan: Map<number, KonvaRobot>;
  private robotsMagenta: Map<number, KonvaRobot>;
  private connected: Boolean;
  private MachineMap: Map<number, KonvaMachine>;

  constructor(private http: HttpClient, private zoneService: ZonesService) {
    this.Client = http;
    this.ZoneService = zoneService;
    const connect_interval = interval(2000);
    this.initSubscription = connect_interval.subscribe(() => this.init());
    this.subscription = connect_interval.subscribe();
    this.stage = undefined;
    this.layer = undefined;
    this.robotsCyan = new Map<number, KonvaRobot>;
    this.robotsMagenta = new Map<number, KonvaRobot>;
    this.ZonesMap = new Map<number, KonvaZone>();
    this.MachineMap = new Map<number, KonvaMachine>;
    this.connected = false;
  }

  ngOnInit(): void {
    this.stage = new Konva.Stage({
      container: 'container',
      width: window.innerWidth,
      height: 820,
    });
    this.layer = new Konva.Layer();
    this.stage.add(this.layer);
  }

  init(): void {
    console.log("Init!");
    this.getZonesFromServer();
    this.getRobotDataFromServer();
    this.getMachinesFromServer();
    let i = 0;
    if (this.zonesData != undefined) {
      for (i = 0; i < this.zonesData?.length; i++) {
        var z = this.zonesData[i]
        var color = 'magenta'
        if (z.X >= 7) {
          color = 'cyan'
        }
        this.createZone(z, color)
      }
    }
    if (this.robotsData != undefined) {
      for (i = 0; i < this.robotsData?.length; i++) {
        var robot = this.robotsData[i];
        this.createRobot(robot);
      }
    }
    if (this.machinesData != undefined) {
      for (i = 0; i < this.machinesData?.length; i++) {
        var machine = this.machinesData[i];
        //console.log("Machine = " + machine.Name + " " + machine.Zone);
        this.createMachines(machine);
      }
    }
    if (this.zonesData != undefined && this.robotsData != undefined && this.machinesData != undefined) {
      this.subscription.unsubscribe();
      this.initSubscription.unsubscribe();
      const update_interval = interval(1000);
      this.subscription = update_interval.subscribe(() => this.update());
    }
  }

  update(): void {
    console.log("Update!");
    this.getZonesFromServer();
    this.getRobotDataFromServer();
    this.getMachinesFromServer();
    this.updateZones();
    this.updateRobots();
    this.updateMachines();
  }


  createZone(z: Zone, color: string): void {
    if (this.layer == undefined)
      return;
    this.ZonesMap.set(z.ZoneId, new KonvaZone(z, color, this.layer));
  }

  createRobot(robot: Robot): void {
    if (this.layer == undefined)
      return;
    var konvaZone = this.ZonesMap.get(robot.CurrentZone.ZoneId);
    if (konvaZone == undefined)
      return;
    if (robot.TeamColor == TeamColor.CYAN) {
      this.robotsCyan.set(robot.JerseyNumber, new KonvaRobot(robot, this.layer, konvaZone));
    } else {
      this.robotsMagenta.set(robot.JerseyNumber, new KonvaRobot(robot, this.layer, konvaZone));
    }
  }

  createMachines(machine: Machine): void {
    var konvaZone = this.ZonesMap.get(machine.Zone);

    if (konvaZone == undefined || this.layer == undefined)
      return;
    this.MachineMap.set(machine.InternalId, new KonvaMachine(machine, konvaZone, this.layer));
  }

  getZonesFromServer(): void {
    this.ZoneService.getZones().subscribe((zones) => this.zonesData = zones);
  }

  getRobotDataFromServer(): void {
    this.ZoneService.getRobots().subscribe((robots) => this.robotsData = robots);
  }

  getMachinesFromServer(): void {
    this.ZoneService.getMachines().subscribe((machine) => this.machinesData = machine);
  }

  private updateZones() {
    if (this.zonesData == undefined)
      return;
    for (let i = 0; i < this.zonesData.length; i++) {
      let zoneData = this.zonesData[i];
      let Zone = this.ZonesMap.get(zoneData.ZoneId)
      if (Zone == undefined)
        continue;
      Zone.Update(zoneData);
    }
  }

  private updateMachines() {
    if (this.machinesData == undefined)
      return;
    for (let i = 0; i < this.machinesData.length; i++) {
      let machine = this.machinesData[i];
      let konvaMachine = this.MachineMap.get(machine.InternalId);
      let newZone = this.ZonesMap.get(machine.Zone);
      if (newZone == undefined || konvaMachine == undefined)
        continue;
      konvaMachine.Update(machine, newZone);
    }
  }

  private updateRobots() {
    if (this.robotsData == undefined)
      return;
    for (let i = 0; i < this.robotsData.length; i++) {
      let robotdata = this.robotsData[i];
      let Robot: KonvaRobot | undefined;
      if (robotdata.TeamColor == TeamColor.CYAN) {
        Robot = this.robotsCyan.get(robotdata.JerseyNumber);
      } else {
        Robot = this.robotsMagenta.get(robotdata.JerseyNumber);
      }
      let newZone = this.ZonesMap.get(robotdata.CurrentZone.ZoneId);
      if (Robot == undefined || newZone == undefined)
        return;
      Robot.Update(robotdata, newZone);
    }
  }


}
