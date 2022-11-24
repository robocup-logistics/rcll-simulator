import {Product} from "./Product";

export interface Robot{
  RobotName: string,
  TeamName: string,
  JerseyNumber:number,
  TeamColor: number,
  CurrentTask: string,
  Machines: string[],
  Position: Position,
  TaskDescription:string,
  CurrentZone: Zone,
  nextZone: Zone,
  FinishedTasksList:string[],
  HeldProduct: Product
}
export interface Position{
  X:number,
  Y:number,
  Orientation:number,
}
export interface Zone{
  ZoneId: number,
  Orientation: number,
  X:number,
  Y:number,
  GetsMovedTo: boolean
}
