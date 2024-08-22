import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeAdminComponent } from './change.component';

describe('ChangeComponent', () => {
  let component: ChangeAdminComponent;
  let fixture: ComponentFixture<ChangeAdminComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChangeAdminComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ChangeAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
