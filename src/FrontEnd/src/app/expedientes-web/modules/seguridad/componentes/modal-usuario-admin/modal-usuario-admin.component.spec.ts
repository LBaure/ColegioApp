import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalUsuarioAdminComponent } from './modal-usuario-admin.component';

describe('ModalUsuarioAdminComponent', () => {
  let component: ModalUsuarioAdminComponent;
  let fixture: ComponentFixture<ModalUsuarioAdminComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModalUsuarioAdminComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalUsuarioAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
