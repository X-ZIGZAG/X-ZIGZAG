import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetClientComponent } from './get-client.component';

describe('GetClientComponent', () => {
  let component: GetClientComponent;
  let fixture: ComponentFixture<GetClientComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GetClientComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GetClientComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
