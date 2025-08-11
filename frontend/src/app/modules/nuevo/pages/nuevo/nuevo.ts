import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';

import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Subscription } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';

import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

import { DataApi } from '../../../../core/services/data-api';
import { TitleStore } from '../../../shell/title-store';
import { NavigationStore } from '../../../shell/navigation-store';
import { NotificationStore } from '../../../../core/services/notification-store';
import { FormDirty } from '../../../../core/guard/unsavedChanges';
import { LavadoStatus, LavadoVO } from '../../../../core/models/Lavado';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';

@Component({
  selector: 'cm-nuevo',
  templateUrl: './nuevo.html',
  styleUrl: './nuevo.scss',
  imports: [
    CommonModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatIconModule,
    TranslateModule,
    MatButtonModule,
  ],
})
export class Nuevo implements OnInit, OnDestroy, FormDirty {
  private titleStore = inject(TitleStore);
  private dataApi = inject(DataApi);
  private fb = inject(FormBuilder);
  private dialog = inject(MatDialog);
  private notificationStore = inject(NotificationStore);
  private navigation = inject(NavigationStore);

  subs: Subscription = new Subscription();
  operarios$ = this.dataApi.getOperarios();
  maquinas$ = this.dataApi.getMaquinas();

  form!: FormGroup;
  isAddButtonDisabled = true;
  prots = signal<string[]>([]);
  protected showLavadoIniciado = false;

  constructor() {
    this.form = this.fb.group({
      operario: [null, Validators.required],
      maquina: ['', Validators.required],
      manualInputProt: [''],
      observaciones: [''],
    });
  }
  isFormDirty(): boolean {
    return this.form.dirty;
  }

  ngOnInit() {
    this.titleStore.updateTitle('TITLES.NUEVO_LAVADO');
    this.valueChangesManualInput();
  }
  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }
  private valueChangesManualInput() {
    this.subs.add(
      this.form.get('manualInputProt')!.valueChanges.subscribe((value) => {
        if (value && value.trim() !== '') {
          this.isAddButtonDisabled = false;
        } else {
          this.isAddButtonDisabled = true;
        }
      }),
    );
  }
  onScanQR() {
    // Implementar la lógica para escanear un código QR
    console.log('onScanQR');
  }
  onAddManualCode() {
    if (
      this.prots().findIndex(
        (prot) => prot === this.form.value.manualInputProt.trim(),
      ) === -1
    ) {
      this.addProt();
    } else {
      this.notificationStore.warn('M_NUEVO.PROT_YA_EXISTE');
    }
  }
  private addProt() {
    this.prots.update((prots) => [
      ...prots,
      this.form.get('manualInputProt')?.value.trim(),
    ]);
    this.form.patchValue({ manualInputProt: '' });
  }
  onRemoveProt(prot: string) {
    this.prots.update((prots) => prots.filter((p) => p !== prot));
  }
  onIniciarLavado() {
    if (this.form.valid && this.prots().length > 0) {
      this.showConfirmationDialog();
    } else {
      this.notificationStore.warn('M_NUEVO.FORM_INCOMPLETO');
    }
  }
  private showConfirmationDialog() {
    this.dialog
      .open(ConfirmDialog, {
        data: {
          title: 'M_NUEVO.INICIO_LAVADO',
          text: 'M_NUEVO.CONFIRM_INICIO_LAVADO',
          btn_text: false,
        },
        panelClass: 'dialog',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.addLavado(this.lavado());
        }
      });
  }
  private lavado(): LavadoVO {
    return {
      washingId: 0,
      machine: this.form.value.maquina,
      startDate: new Date(),
      endDate: null,
      startUserId: this.form.value.operario,
      endUserId: null,
      status: LavadoStatus.IN_PROGRESS,
      protId: '', //FIXME: Por consultar (añado el array más abajo)
      loteNumber: '', //Por consultar
      bagNumber: '', // Por consultar
      startObservation: this.form.value.observaciones,
      finishObservation: '',
      prots: this.prots(), //Por consultar (array de prots??)
    };
  }
  private addLavado(lavado: LavadoVO) {
    this.subs.add(
      this.dataApi.iniciarLavado(lavado).subscribe({
        next: () => {
          this.notificationStore.success('M_NUEVO.INICIO_LAVADO_OK');
          this.statusLavadoIniciado();
        },
        error: (error) => {
          console.error('Error al iniciar el lavado:', error);
          this.notificationStore.error('M_NUEVO.INICIO_LAVADO_ERR');
        },
      }),
    );
  }
  private goToInicio() {
    this.navigation.goToInicio();
  }
  onVolverInicio() {
    // this.form.markAsPristine();
    this.goToInicio();
  }
  private statusLavadoIniciado() {
    this.titleStore.updateTitle('TITLES.LAVADO_INICIADO');
    window.scrollTo(0, 0);
    this.showLavadoIniciado = true;
    this.form.disable();
    this.form.markAsPristine();
  }
}
