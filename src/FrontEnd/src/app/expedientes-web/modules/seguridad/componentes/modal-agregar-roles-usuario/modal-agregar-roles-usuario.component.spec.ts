import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalAgregarRolesUsuarioComponent } from './modal-agregar-roles-usuario.component';

describe('ModalAgregarRolesUsuarioComponent', () => {
  let component: ModalAgregarRolesUsuarioComponent;
  let fixture: ComponentFixture<ModalAgregarRolesUsuarioComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModalAgregarRolesUsuarioComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalAgregarRolesUsuarioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
