import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardFasesLateralComponent } from './dashboard-fases-lateral.component';

describe('DashboardFasesLateralComponent', () => {
  let component: DashboardFasesLateralComponent;
  let fixture: ComponentFixture<DashboardFasesLateralComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DashboardFasesLateralComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DashboardFasesLateralComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
