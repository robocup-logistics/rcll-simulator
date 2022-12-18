import {Product} from "./Product";

export interface Machine {
  InternalId: number,
  Name: string,
  Port: number,
  ProductAtIn: Product,
  ProductAtOut: Product,
  ProductOnBelt: Product
  Rotation: number,
  TaskDescription: string,
  Zone: number;
  YellowLight: Light;
  RedLight: Light;
  GreenLight: Light;
  SlideCount: number;
  //GetsMovedTo:boolean;
}

export interface Light {
  LightColor: LightColor,
  LightOn: LightState
}

export enum LightColor {
  RED = 0,
  YELLOW = 1,
  GREEN = 2,
}

export enum LightState {
  OFF = 0,
  ON = 1,
  BLINK = 2,
}
