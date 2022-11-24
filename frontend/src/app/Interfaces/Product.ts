export interface Product{
  ID: number,
  Complexity: number,
  RingCount: number,
  Base: Base,
  Cap: Cap,
  RingList: Ring[]
}
export interface Base{
  BaseColor:number,
}
export interface Cap{
  CapColor:number,
}
export interface Ring{
  RingColor:number,
}
