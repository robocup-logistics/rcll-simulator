import {Component, Injectable, OnInit} from '@angular/core';
import {Zone} from "../Interfaces/Zone";
import Konva from "konva";
import {KonvaZone} from "./KonvaZone";
import {Machine} from "../Interfaces/Machine";
import {Robot} from "../Interfaces/Robot";
import {TeamColor} from "./test-component.component"
import {KonvaProduct} from "./KonvaProduct";

export class KonvaRobot {

  public Group: Konva.Group;
  public Rect: Konva.Circle;
  public Text: Konva.Text;
  public TaskText: Konva.Text;
  private Color: string;
  private Product: KonvaProduct;

  //public ZoneId: number;

  constructor(robot: Robot, layer: Konva.Layer, initialZone: KonvaZone) {
    if (robot.TeamColor == TeamColor.CYAN) {
      this.Color = "darkcyan";
    } else {
      this.Color = "purple";
    }
    this.Group = new Konva.Group(
      {
        x: initialZone.Group.x(),
        y: initialZone.Group.y(),
        width: initialZone.Group.width(),
        height: initialZone.Group.height(),
      }
    )
    this.Rect = new Konva.Circle({
      x: this.Group.width() / 2,
      y: this.Group.height() / 2,
      radius: (this.Group.width() * 0.8) / 2,
      fill: this.Color,
      stroke: 'black',
      strokeWidth: 2,
    })
    this.Text = new Konva.Text({
      x: 0,
      y: 0,
      width: this.Group.width(),
      height: this.Group.height(),
      fill: "white",
      text: robot.RobotName,
      fontSize: 16,
      verticalAlign: "middle",
      align: "center"
    });
    this.TaskText = new Konva.Text({
      x: 0,
      y: this.Group.width() / 3,
      width: this.Group.width(),
      height: this.Group.height() * 2 / 3,
      fill: "white",
      text: "Idle",
      fontSize: 10,
      verticalAlign: "middle",
      align: "center"
    });
    this.Group.add(this.Rect);
    this.Group.add(this.Text);
    this.Group.add(this.TaskText);
    layer.add(this.Group);
    this.Product = new KonvaProduct(robot.HeldProduct,this.Group.x() + this.Group.width() / 2 - 4,this.Group.y() + 14, this, layer, this.Group);
  }

  public Update(robotData: Robot, newZone: KonvaZone) {
    this.Group.setPosition(newZone.Group.getPosition());
    this.TaskText.text(robotData.TaskDescription);
    this.Product.Update(robotData.HeldProduct,this);
  }
}
