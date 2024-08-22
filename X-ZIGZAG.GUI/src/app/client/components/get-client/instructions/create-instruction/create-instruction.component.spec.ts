import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateInstructionComponent } from './create-instruction.component';

describe('CreateInstructionComponent', () => {
  let component: CreateInstructionComponent;
  let fixture: ComponentFixture<CreateInstructionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateInstructionComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CreateInstructionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
