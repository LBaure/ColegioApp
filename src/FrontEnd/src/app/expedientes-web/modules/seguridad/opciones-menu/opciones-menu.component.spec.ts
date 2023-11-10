import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OpcionesMenuComponent } from './opciones-menu.component';

describe('OpcionesMenuComponent', () => {
  let component: OpcionesMenuComponent;
  let fixture: ComponentFixture<OpcionesMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OpcionesMenuComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OpcionesMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
