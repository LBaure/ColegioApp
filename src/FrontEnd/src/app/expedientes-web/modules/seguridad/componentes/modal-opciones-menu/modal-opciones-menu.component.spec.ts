import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalOpcionesMenuComponent } from './modal-opciones-menu.component';

describe('ModalOpcionesMenuComponent', () => {
  let component: ModalOpcionesMenuComponent;
  let fixture: ComponentFixture<ModalOpcionesMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModalOpcionesMenuComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalOpcionesMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
