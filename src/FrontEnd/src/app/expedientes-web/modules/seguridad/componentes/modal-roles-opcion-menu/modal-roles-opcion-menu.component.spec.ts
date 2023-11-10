import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalRolesOpcionMenuComponent } from './modal-roles-opcion-menu.component';

describe('ModalRolesOpcionMenuComponent', () => {
  let component: ModalRolesOpcionMenuComponent;
  let fixture: ComponentFixture<ModalRolesOpcionMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModalRolesOpcionMenuComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalRolesOpcionMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
