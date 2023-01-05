import {Component, Injectable, OnInit} from '@angular/core';
import {Zone} from "../Interfaces/Zone";
import Konva from "konva";

export class KonvaZone{

  public Group: Konva.Group;
  public Rect: Konva.Rect;
  public Text: Konva.Text;
  public ZoneId: number;
  private DefaultColor: string;
  private HighlightColor: string;
  private Prefix: string;
  private Width: number;
  private Height: number;
  constructor(z: Zone, color: string, layer: Konva.Layer) {
    if(color.includes("cyan"))
    {
      this.DefaultColor = "lightblue";
      this.Prefix = "C"
    }
    else {
      this.DefaultColor = "pink";
      this.Prefix = "M"
    }
    this.HighlightColor = "yellow"
    this.ZoneId = z.ZoneId;
    var offsetX = 0;
    this.Width = 100;
    this.Height = 100;
    this.Group = new Konva.Group(
      {
        x: offsetX + z.X * this.Width,
        y: 800 -  z.Y *  this.Height,
        width:  this.Width,
        height:  this.Height,
      }
    )
    this.Rect = new Konva.Rect({
      x: 0,
      y: 0,
      width: this.Group.width(),
      height: this.Group.height(),
      fill: this.DefaultColor,
      stroke: 'black',
      strokeWidth: 2
    });
    this.Text = new Konva.Text({
      x: 2,
      y: 2,
      width: 40,
      height: 20,
      text: this.Prefix + (z.ZoneId % 100)
    })
    this.Group.add(this.Rect);
    this.Group.add(this.Text);
    if (layer != undefined) {
      layer.add(this.Group);
    }
  }
  public Update(zoneData: Zone){
    if (zoneData.GetsMovedTo) {
      this.Rect.fill(this.HighlightColor);
    } else {
      this.Rect.fill(this.DefaultColor);
    }
  }

}
