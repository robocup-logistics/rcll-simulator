import Konva from "konva";
import {KonvaZone} from "./KonvaZone";
import {LightState, Machine} from "../Interfaces/Machine";
import {KonvaProduct} from "./KonvaProduct";

const rotatePoint = ({x, y}: { x: number; y: number },
                     rad: number) => {
  const rcos = Math.cos(rad);
  const rsin = Math.sin(rad);
  return {x: x * rcos - y * rsin, y: y * rcos + x * rsin};
};

enum MachineType {
  BS = 0,
  CS = 1,
  DS = 2,
  RS1 = 3,
  RS2 = 4,
  SS = 5,
}

export class KonvaMachine {

  public Group: Konva.Group;
  public Text: Konva.Text;
  public BodyRect: Konva.Rect;
  //@ts-ignore
  public ImageRect: Konva.Rect;
  public TaskText: Konva.Text;
  private Zone: KonvaZone;
  private readonly Type: MachineType;
  private ProductAtInRect: KonvaProduct;
  private ProductAtOutRect: KonvaProduct;
  private ProductOnBeltRect: KonvaProduct;
  private CurrentRotation: number;
  //@ts-ignore
  private SlideCnt: Konva.Text;
  //@ts-ignore
  private SlideCntRect: Konva.Rect;
  constructor(machine: Machine, konvaZone: KonvaZone, layer: Konva.Layer) {
    this.Group = new Konva.Group({
      x: konvaZone.Group.x(),
      y: konvaZone.Group.y(),
      width: konvaZone.Group.width(),
      height: konvaZone.Group.height(),
    });
    this.CurrentRotation = machine.Rotation;
    let color = "";
    if (machine.Name.includes("M-")) {
      color = "magenta";
    } else {
      color = "cyan";
    }
    this.Type = this.GetType(machine.Name);

    this.Text = new Konva.Text({
      x: 0,
      y: 0,
      width: this.Group.width(),
      height: this.Group.height() / 8,
      text: machine.Name.split("-")[1],
      fontSize: 18,
      fill: "black",
      verticalAlign: "top",
      align: "center",
    })
    this.TaskText = new Konva.Text({
      x: 0,
      y: this.Group.height() * 7 / 8,
      width: this.Group.width(),
      height: this.Group.height() * 1 / 8,
      fill: "black",
      text: "Idle",
      fontSize: 15,
      verticalAlign: "bottom",
      align: "center"
    });

    var ImageResolutionX = 90;
    var imageResolutionY = 150;
    let resolution = 10;
    let widthScale = 3 / resolution;
    let heightScale = 1 / resolution;

    this.BodyRect = new Konva.Rect({
      x: this.Group.width() * widthScale,
      y: this.Group.width() * heightScale,
      width: this.Group.width() * (1 - 2 * widthScale),
      height: this.Group.width() * (1 - 2 * heightScale),
      fill: color,
      stroke: 'gray',
      strokeWidth: 1,
    });

    let imageObj = new Image();
    imageObj.onload = () => {
      let img = new Konva.Image({
        x: this.Group.width() * widthScale,
        y: this.Group.width() * heightScale,
        width: this.Group.width() * (1 - 2 * widthScale),
        height: this.Group.width() * (1 - 2 * heightScale),
        image: imageObj,
      });
      this.ImageRect = new Konva.Rect({
        x: this.Group.width() * widthScale,
        y: this.Group.width() * heightScale,
        width: this.Group.width() * (1 - 2 * widthScale),
        height: this.Group.width() * (1 - 2 * heightScale),
        fillPatternImage: imageObj,
        fillPatternScale: {x: this.BodyRect.width() / ImageResolutionX, y: this.BodyRect.height() / imageResolutionY}
      });
      this.rotateAroundCenter(this.ImageRect, -machine.Rotation);
      this.Group.add(this.ImageRect);
    }
    this.ProductOnBeltRect = new KonvaProduct(machine.ProductOnBelt, this.Group.x() + 10, this.Group.y() + 10, this, layer, this.Group);
    this.ProductAtOutRect = new KonvaProduct(machine.ProductOnBelt, this.Group.x() + 30, this.Group.y() + 10, this, layer, this.Group);
    this.ProductAtInRect = new KonvaProduct(machine.ProductOnBelt, this.Group.x() + 50, this.Group.y() + 10, this, layer, this.Group);

    imageObj.src = this.GetAssetPath(this.Type);
    this.rotateAroundCenter(this.BodyRect, -machine.Rotation);

    if(this.Type == MachineType.RS1 || this.Type == MachineType.RS2)
    {

      this.SlideCnt = new Konva.Text({
        x: this.Group.width()* 8 / 12,
        y: this.Group.height() * 0 / 12,
        width:  this.Group.width() * 4 / 12,
        height: this.Group.height() * 1 / 8,
        fill: "black",
        text: "Cnt: 00",
        fontSize: 8,
        verticalAlign: "bottom",
        align: "center"
      });
      /*this.SlideCntRect = new Konva.Rect({
        x: this.Group.width()* 8 / 12,
        y: this.Group.height() * 0 / 12,
        width: this.Group.width() * 4 / 12,
        height: this.Group.height() * 1 / 8,
        fill: "white",
        verticalAlign: "bottom",
        align: "center",
        stroke: 'black',
        strokeWidth: 1,
      });*/
      this.Group.add(this.SlideCnt);
      //this.Group.add(this.SlideCntRect);
    }

    //this.rotateAroundCenter()
    this.Group.add(this.BodyRect);
    this.Group.add(this.Text);
    this.Group.add(this.TaskText);
    layer.add(this.Group);
    this.Zone = konvaZone;
  }


// will work for shapes with top-left origin, like rectangle
  rotateAroundCenter(node: Konva.Shape, rotation: number) {
    //current rotation origin (0, 0) relative to desired origin - center (node.width()/2, node.height()/2)
    const topLeft = {x: -node.width() / 2, y: -node.height() / 2};
    const current = rotatePoint(topLeft, node.rotation() * Math.PI / 180);
    const rotated = rotatePoint(topLeft, rotation * Math.PI / 180);
    const dx = rotated.x - current.x,
      dy = rotated.y - current.y;

    node.rotation(rotation);
    node.x(node.x() + dx);
    node.y(node.y() + dy);
  }


  Update(machinesData: Machine, zone: KonvaZone) {
    if(this.CurrentRotation != machinesData.Rotation)
    {
      this.rotateAroundCenter(this.ImageRect, -machinesData.Rotation);
      this.rotateAroundCenter(this.BodyRect, -machinesData.Rotation);
      this.CurrentRotation = machinesData.Rotation;
    }
    this.Group.setPosition(zone.Group.position());
    this.TaskText.text(machinesData.TaskDescription);
    this.ProductOnBeltRect.Update(machinesData.ProductOnBelt, this);
    this.ProductAtInRect.Update(machinesData.ProductAtIn, this);
    this.ProductAtOutRect.Update(machinesData.ProductAtOut, this);
    if(machinesData.RedLight.LightOn == LightState.ON && machinesData.YellowLight.LightOn == LightState.OFF && machinesData.GreenLight.LightOn == LightState.OFF)
    {
      this.BodyRect.stroke("red");
      this.BodyRect.strokeWidth(5);
    }
    else if (machinesData.TaskDescription.toLowerCase() == "idle") {
      this.BodyRect.stroke("gray");
      this.BodyRect.strokeWidth(1);
    } else {
      this.BodyRect.stroke("green");
      this.BodyRect.strokeWidth(5);
    }
    if(this.Type == MachineType.RS1 || this.Type == MachineType.RS2)
    {
      this.SlideCnt.text("Cnt: " + this.addLeadingZeros(machinesData.SlideCount,2));
      //this.SlideCntRect.moveToTop();
      this.SlideCnt.moveToTop();
    }
  }

  addLeadingZeros(num: number, totalLength: number): string {
    return String(num).padStart(totalLength, '0');
  }

  GetType(name: string): MachineType {
    if (name.includes("BS")) {
      return MachineType.BS;
    } else if (name.includes("RS1")) {
      return MachineType.RS1;
    } else if (name.includes("RS2")) {
      return MachineType.RS2;
    } else if (name.includes("DS")) {
      return MachineType.DS;
    } else if (name.includes("CS")) {
      return MachineType.CS;
    } else {
      return MachineType.SS;
    }
  }

  GetAssetPath(type: MachineType): string {
    switch (this.Type) {
      case MachineType.BS:
        return './assets/machine_bs.png';
      case MachineType.CS:
        return './assets/machine_cs.png';
      case MachineType.RS1:
        return './assets/machine_rs1.png';
      case MachineType.RS2:
        return './assets/machine_rs2.png';
      case MachineType.DS:
        return './assets/machine_ds.png';
      default:
        return './assets/machine.png';
    }

  }
}
