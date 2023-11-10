import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { YearDirective } from './directivas/year.directive'



@NgModule({
  declarations: [
    YearDirective
  ],
  imports: [
    CommonModule,
    HttpClientModule
  ]
})
export class UtilsModule { }
