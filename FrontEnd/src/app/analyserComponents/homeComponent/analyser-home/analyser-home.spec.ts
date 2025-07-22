import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AnalyserHome } from './analyser-home';

describe('AnalyserHome', () => {
  let component: AnalyserHome;
  let fixture: ComponentFixture<AnalyserHome>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AnalyserHome]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AnalyserHome);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
