import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-forms',
  templateUrl: './forms.component.html',
  styleUrls: ['./forms.component.css']
})
export class FormsComponent implements OnInit {

  model: string = ''
  fileModel: any;

  colorTheme = 'theme-green';
  bsConfig: Partial<BsDatepickerConfig> = Object.assign({}, { dateInputFormat: 'DD/MM/YYYY'});
  bsConfigTime: Partial<BsDatepickerConfig> = Object.assign({}, {withTimepicker: true, rangeInputFormat : 'MMMM Do YYYY, hh:mm:ss', dateInputFormat: 'MMMM Do YYYY, hh:mm:ss' })
  DataPickerTimePicker : boolean = true;
  constructor() { }
  ngOnInit(): void {
  }

  save (evt: any) {
    console.log("fileModel", evt)
    console.log("model", this.model);

  }

  applyTheme(pop: any) {
    // create new object on each property change
    // so Angular can catch object reference change
    this.bsConfig = Object.assign({}, { containerClass: this.colorTheme });
    setTimeout(() => {
      pop.show();
    });
  }

}
