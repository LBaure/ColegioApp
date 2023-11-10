import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PublicAppRoutingModule } from './public-app-routing.module';
import { HomeComponent } from './home/home.component';
import { ComponentesHtmlModule } from '../componentes-html/componentes-html.module';


@NgModule({
  declarations: [
    HomeComponent
  ],
  imports: [
    CommonModule,
    PublicAppRoutingModule,
    ComponentesHtmlModule
  ]
})
export class PublicAppModule { }
