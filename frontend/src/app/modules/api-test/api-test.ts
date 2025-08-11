import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { WashingApiService, NewWashDto, WashingResponseDto } from '../../core/services/washing-api.service';

@Component({
  selector: 'cm-api-test',
  standalone: true,
  templateUrl: './api-test.html',
  styleUrl: './api-test.scss',
  imports: [CommonModule, ReactiveFormsModule]
})
export class ApiTest implements OnInit {
  private fb = inject(FormBuilder);
  private washingApi = inject(WashingApiService);

  form = this.fb.group({
    machineId: [1, Validators.required],
    startUserId: [1, Validators.required],
    startObservation: [''],
    protId: ['PROT001', Validators.required],
    batchNumber: ['NL01', Validators.required],
    bagNumber: ['01/02', Validators.required]
  });

  activeWashes: WashingResponseDto[] = [];

  ngOnInit() {
    this.loadActive();
  }

  start() {
    if (this.form.valid) {
      const dto: NewWashDto = {
        machineId: this.form.value.machineId!,
        startUserId: this.form.value.startUserId!,
        startObservation: this.form.value.startObservation || undefined,
        protEntries: [{
          protId: this.form.value.protId!,
          batchNumber: this.form.value.batchNumber!,
          bagNumber: this.form.value.bagNumber!
        }]
      };
      this.washingApi.startWash(dto).subscribe(() => this.loadActive());
    }
  }

  loadActive() {
    this.washingApi.getActiveWashes().subscribe(data => this.activeWashes = data);
  }
}
