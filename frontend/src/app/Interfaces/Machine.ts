import {Product} from "./Product";

export interface Machine{
  InternalId: number,
  Name:string,
  Port: number,
  ProductAtIn: Product,
  ProductAtOut: Product,
  ProductOnBelt: Product
  Rotation:number,
  TaskDescription: string,
  Zone: number;
  //GetsMovedTo:boolean;
}
