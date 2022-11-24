import { Component, OnInit, Input } from '@angular/core';
import { Zone } from "../Interfaces/Zone"
@Component({
  selector: 'app-zone',
  templateUrl: './zone.component.html',
  styleUrls: ['./zone.component.css']
})
export class ZoneComponent implements OnInit {
  @Input() zone! : Zone;
  constructor() { }

  ngOnInit(): void {
  }

}
