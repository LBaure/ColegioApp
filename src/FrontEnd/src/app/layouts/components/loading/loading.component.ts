import { Component } from '@angular/core';
import { LoadingService } from './loading.service';

@Component({
  selector: 'app-loading',
  templateUrl: 'loading.component.html',
  styleUrls: ['./loading.component.css']
})
export class LoadingComponent {
  isLoading$ = this.loadingSvc.isLoading$;
  constructor(private readonly loadingSvc : LoadingService) { }
}
