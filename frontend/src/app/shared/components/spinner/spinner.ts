import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { SpinnerStore } from '../../../core/services/spinner-store';

@Component({
  selector: 'cm-spinner',
  imports: [CommonModule],
  templateUrl: './spinner.html',
  styleUrl: './spinner.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Spinner {
  private readonly spinnerStore = inject(SpinnerStore);
  public show = this.spinnerStore.spinner;

  public readonly colorClass = 'spinner--blue';
}
