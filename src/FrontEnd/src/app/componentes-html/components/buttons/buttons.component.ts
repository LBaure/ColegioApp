import { Component, HostListener, OnInit } from '@angular/core';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-buttons',
  templateUrl: './buttons.component.html',
  styleUrls: ['./buttons.component.css']
})
export class ButtonsComponent implements OnInit {
  userActivity : any;
  userInactive: Subject<any> = new Subject();

  constructor() {
    this.setTimeout();
    this.userInactive.subscribe(() => console.log('user has been inactive for 8s'));
   }

  ngOnInit(): void {
  }

  setTimeout() {
    this.userActivity = setTimeout(() => this.userInactive.next(undefined),  1 * 60 * 1000);
  }

  @HostListener('window:mousemove') refreshUserState() {
    clearTimeout(this.userActivity);
    this.setTimeout();
  }
}
