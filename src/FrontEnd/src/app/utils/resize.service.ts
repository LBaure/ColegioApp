import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject, debounceTime, fromEvent, takeUntil } from 'rxjs';
import { SCREEN_SIZE } from './screen-size.enum';

@Injectable({
  providedIn: 'root'
})
export class ResizeService {

  private _unsubscriber$: Subject<any> = new Subject();
  public screenWidth$: BehaviorSubject<SCREEN_SIZE> = new BehaviorSubject<SCREEN_SIZE>(SCREEN_SIZE.LG)
  public mediaBreakpoint$: BehaviorSubject<SCREEN_SIZE> = new BehaviorSubject<SCREEN_SIZE>(SCREEN_SIZE.LG)

  constructor() { }

  init() {
    this._setScreenWidth(window.innerWidth);
    fromEvent(window, 'resize')
      .pipe(
        debounceTime(0),
        takeUntil(this._unsubscriber$)
      ).subscribe((evt: any) => {
        this._setScreenWidth(evt.target.innerWidth);
      });
  }

  ngOnDestroy() {
    this._unsubscriber$.next(null);
    this._unsubscriber$.complete();
  }

  private _setScreenWidth(width: number): void {
    let sizeWidth :  SCREEN_SIZE = SCREEN_SIZE.LG;
    if (width < 576) {
      sizeWidth = SCREEN_SIZE.XS;
    } else if (width > 576 && width < 767){
      sizeWidth = SCREEN_SIZE.SM;
    } else if (width >= 768 && width < 991){
      sizeWidth = SCREEN_SIZE.MD;
    } else if (width >= 992 && width < 1199){
      sizeWidth = SCREEN_SIZE.LG;
    } else if (width >= 1200){
      sizeWidth = SCREEN_SIZE.XL;
    }
    this.screenWidth$.next(sizeWidth);
  }

  public observar(): Observable<SCREEN_SIZE>{
   return this.screenWidth$.asObservable();
  }
}
