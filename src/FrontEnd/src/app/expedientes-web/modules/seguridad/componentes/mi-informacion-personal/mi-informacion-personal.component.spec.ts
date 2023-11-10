import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MiInformacionPersonalComponent } from './mi-informacion-personal.component';

describe('MiInformacionPersonalComponent', () => {
  let component: MiInformacionPersonalComponent;
  let fixture: ComponentFixture<MiInformacionPersonalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MiInformacionPersonalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MiInformacionPersonalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
