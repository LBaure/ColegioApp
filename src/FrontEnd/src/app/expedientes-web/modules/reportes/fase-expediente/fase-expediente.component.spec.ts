import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FaseExpedienteComponent } from './fase-expediente.component';

describe('FaseExpedienteComponent', () => {
  let component: FaseExpedienteComponent;
  let fixture: ComponentFixture<FaseExpedienteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FaseExpedienteComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FaseExpedienteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
