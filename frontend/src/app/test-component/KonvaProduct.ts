import Konva from "konva";
import {BaseColor, CapColor, RingColors} from "./test-component.component"
import {Product} from "../Interfaces/Product";
import {KonvaMachine} from "./KonvaMachine";
import {KonvaRobot} from "./KonvaRobot";

export class KonvaProduct {

  public OwnerGroup: Konva.Group;
  public Rect: Konva.Rect;
  //@ts-ignore
  public BaseRect: Konva.Rect;
  //@ts-ignore
  public RingRects: Konva.Rect[];
  //@ts-ignore
  public CapRect: Konva.Rect;
  private readonly Width: number;
  private readonly PositionX: number;
  private readonly PositionY: number;

  constructor(product: Product, positionX: number, positionY: number, holder: KonvaRobot | KonvaMachine, layer: Konva.Layer, ownerGroup : Konva.Group) {
    let full_height = 20;
    let height = 3;
    this.Width = 8;
    this.PositionX = positionX;
    this.PositionY = positionY;

    this.OwnerGroup = ownerGroup;
    this.Rect = new Konva.Rect({
      x: 0,
      y: 0,
      width: this.Width,
      height: full_height,
      fill: "black",
    });

    this.OwnerGroup.add(this.Rect);
    let y = 2;
    this.CapRect = this.CreateRectangle(2, y, height / 2, this.GetCapColor(CapColor.CapGrey)).visible(false);
    this.OwnerGroup.add(this.CapRect);
    y += height / 2;
    this.RingRects = [];
    for (let i = 0; i < 3; i++) {
      this.RingRects.push(this.CreateRectangle(2, y, height, this.GetRingColor(RingColors.RingOrange)).visible(false));
      this.OwnerGroup.add(this.RingRects[i]);
      full_height -= height;
      y += height;
    }
    this.BaseRect = this.CreateRectangle(2, y, 2 * height, this.GetBaseColor(BaseColor.BaseSilver)).visible(false);
    this.OwnerGroup.add(this.BaseRect);
  }

  public Update(product: Product, holder: KonvaMachine | KonvaRobot) {
    /*this.Group.x(holder.Group.x() + holder.Group.width()/2 - this.Width/2);
    this.Group.y(holder.Group.y());*/
    if (product == null) {
      this.Rect.visible(false);
      this.BaseRect.visible(false);
      this.RingRects.forEach(function (value) {
        value.visible(false);
      })
      this.CapRect.visible(false);
    } else {
      if(product.Base != null)
      {
        this.Rect.visible(true);
        this.Rect.moveToTop();
        this.BaseRect.fill(this.GetBaseColor(product.Base.BaseColor));
        this.BaseRect.visible(true);
        this.BaseRect.moveToTop();
      }
      for(let i = 0; i < product.RingCount; i++)
      {
        this.RingRects[i].fill(this.GetRingColor(product.RingList[i].RingColor));
        this.RingRects[i].visible(true);
        this.RingRects[i].moveToTop();
      }
      if(product.Cap != null)
      {
        this.CapRect.fill(this.GetCapColor(product.Cap.CapColor));
        this.CapRect.visible(true);
        this.CapRect.moveToTop();
      }
    }
  }

  CreateRectangle(x: number, y: number, height: number, color: string): Konva.Rect {
    return new Konva.Rect({
      x: x,
      y: y,
      width: this.Width - 2 * x,
      height: height,
      fill: color,
      stroke: "black",
      strokeWidth: 0.5
    });
  }

  GetBaseColor(color: BaseColor) {
    switch (color) {
      case BaseColor.BaseBlack:
        return "black";
      case BaseColor.BaseRed:
        return "red";
      case BaseColor.BaseSilver:
        return "silver";
      case BaseColor.BaseUncolored:
        return "silver";
    }
  }

  GetCapColor(color: CapColor) {
    switch (color) {
      case CapColor.CapBlack:
        return "black";
      case CapColor.CapGrey:
        return "gray";
    }
  }

  GetRingColor(color: RingColors) {
    switch (color) {
      case RingColors.RingBlue:
        return "blue";
      case RingColors.RingGreen:
        return "green";
      case RingColors.RingYellow:
        return "yellow";
      case RingColors.RingOrange:
        return "orange";
    }
  }
}
