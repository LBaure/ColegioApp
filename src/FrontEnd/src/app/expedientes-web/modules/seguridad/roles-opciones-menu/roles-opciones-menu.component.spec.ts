import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RolesOpcionesMenuComponent } from './roles-opciones-menu.component';

describe('RolesOpcionesMenuComponent', () => {
  let component: RolesOpcionesMenuComponent;
  let fixture: ComponentFixture<RolesOpcionesMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RolesOpcionesMenuComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RolesOpcionesMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
