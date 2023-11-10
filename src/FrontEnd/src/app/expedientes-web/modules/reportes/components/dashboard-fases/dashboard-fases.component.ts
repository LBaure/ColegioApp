import { Component, ElementRef, HostListener, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { Console } from 'console';
import { IFaseExpedienteModelo, IUsuarioExpedienteModelo,  } from 'src/app/expedientes-web/interfaces/dashboard/total-expediente-fase';

@Component({
  selector: 'app-dashboard-fases',
  templateUrl: './dashboard-fases.component.html',
  styleUrls: ['./dashboard-fases.component.css']
})
export class DashboardFasesComponent implements OnInit, OnChanges {
  @Input() items: IUsuarioExpedienteModelo[] = [];
  itemsFases: IUsuarioExpedienteModelo[] =[];

  arr: number[] = [1, 2, 3, 4, 5, 6, 7, 8, 9];
  totalCards: number = 0;
  currentPage: number = 1;
  pagePosition: string = "0%";
  cardsPerPage: number = 0;
  totalPages: number = 0;
  overflowWidth: string = '';
  cardWidth: string = '';
  containerWidth: number = 0;

  @ViewChild("container", { static: true, read: ElementRef })
  container: ElementRef = {} as ElementRef ;
  @HostListener("window:resize") windowResize() {
    let newCardsPerPage = this.getCardsPerPage();
    if (newCardsPerPage != this.cardsPerPage) {
      this.cardsPerPage = newCardsPerPage;
      this.initializeSlider();
      if (this.currentPage > this.totalPages) {
        this.currentPage = this.totalPages;
        this.populatePagePosition();
      }
    }
  }



  ngOnInit(): void {
    this.currentPage = 1;
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.itemsFases = [...this.items]
    if (this.itemsFases.length) {
      this.totalCards = this.itemsFases.length + 1;
      this.cardsPerPage = this.getCardsPerPage();
      this.initializeSlider();
    }
  }



  initializeSlider() {
    this.totalPages = Math.ceil(this.totalCards / this.cardsPerPage);
    this.overflowWidth = `calc(${this.totalPages * 100}% + ${this.totalPages *
      20}px)`;
    this.cardWidth = `calc((${100 / this.totalPages}% - ${this.cardsPerPage *
      20}px) / ${this.cardsPerPage})`;
  }

  getCardsPerPage() {
    let width = this.container.nativeElement.offsetWidth;
    let sizeWidth : number = 0;
    if (width < 576) {
      sizeWidth = 1;
    } else if (width > 576 && width < 767){
      sizeWidth = 2;
    } else if (width >= 768 && width < 991){
      sizeWidth = 3;
    } else if (width >= 992 && width < 1199){
      sizeWidth = 3;
    } else if (width >= 1200){
      sizeWidth = 4;
    }
    return sizeWidth;
  }

  changePage(incrementor:any) {
    this.currentPage += incrementor;
    this.populatePagePosition();
  }

  populatePagePosition() {
    this.pagePosition = `calc(${-100 * (this.currentPage - 1)}% - ${20 *
      (this.currentPage - 1)}px)`;
  }



}
