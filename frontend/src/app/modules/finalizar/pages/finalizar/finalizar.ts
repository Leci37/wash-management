import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
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

import { DataApi } from '../../../../core/services/data-api';
import { TitleStore } from '../../../shell/title-store';
import { NavigationStore } from '../../../shell/navigation-store';
import { NotificationStore } from '../../../../core/services/notification-store';
import { FormDirty } from '../../../../core/guard/unsavedChanges';
import { LavadoStatus, LavadoVO } from '../../../../core/models/Lavado';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { ConfirmPhotoDialog } from '../../../../shared/components/confirm-photo-dialog/confirm-photo-dialog';
import { PhotoVO } from '../../../../core/models/Photo';

@Component({
  selector: 'cm-finalizar',
  imports: [
    CommonModule,
    TranslateModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatIconModule,
  ],
  templateUrl: './finalizar.html',
  styleUrl: './finalizar.scss',
})
export class Finalizar implements OnInit, OnDestroy, FormDirty {
  private readonly activeRoute = inject(ActivatedRoute);
  private titleStore = inject(TitleStore);
  private dataApi = inject(DataApi);
  private fb = inject(FormBuilder);
  private dialog = inject(MatDialog);
  private notificationStore = inject(NotificationStore);
  private navigation = inject(NavigationStore);

  subs: Subscription = new Subscription();
  operarios$ = this.dataApi.getOperarios();

  lavado!: LavadoVO;
  form!: FormGroup;
  photos: PhotoVO[] = []; //FIXME
  maquina!: number;
  protected showLavadoFinalizado = false;

  constructor() {
    this.form = this.fb.group({
      operario: [null, Validators.required],
      observaciones: [''],
    });
  }

  isFormDirty(): boolean {
    return this.form.dirty;
  }
  ngOnInit(): void {
    this.activeRoute.params.subscribe((params) => {
      this.maquina = params['maq'];
      this.titleStore.updateTitle('Finalizar lavado ' + this.maquina);
      this.getLavadoEnCurso();
    });
  }
  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }
  private getLavadoEnCurso() {
    this.subs.add(
      this.dataApi.getLavadoEnCurso(this.maquina).subscribe({
        next: (lavado) => {
          this.lavado = lavado!;
          this.form.patchValue({
            operario: this.lavado.startUserId,
          });
        },
        error: () => {
          this.notificationStore.error('Error al obtener el lavado en curso.');
          this.goToInicio();
        },
      }),
    );
  }
  // onSacarFoto() {
  //   this.photos.push('https://picsum.photos/200/300/?random');
  // }
  onFileSelected(event: Event): void {
    //TOCHECK
    const input = event.target as HTMLInputElement;
    if (input.files?.[0]) {
      const file = input.files[0];
      const reader = new FileReader();
      reader.onload = (e) => {
        const photo: PhotoVO = {
          name: this.photos.length + 1,
          photo: e.target?.result as string | ArrayBuffer | Blob,
        };
        this.showPhotoConfirmationDialog(photo);
        input.value = ''; // Reset input value to allow re-uploading the same file
      };
      reader.readAsDataURL(file);
    }
  }
  private showPhotoConfirmationDialog(photo: PhotoVO) {
    this.dialog
      .open(ConfirmPhotoDialog, {
        panelClass: 'dialog',
        data: {
          photo: photo,
        },
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.photos.push(result);
        }
      });
  }
  onAdjuntarFotos() {
    //TODO
    console.log('Adjuntar fotos');
  }
  onRemoveFoto(photo: PhotoVO) {
    this.photos.splice(this.photos.indexOf(photo), 1);
  }
  onFinalizarLavado() {
    if (this.form.valid && this.photos.length > 0) {
      this.showConfirmationDialog();
    } else {
      this.notificationStore.warn('M_FINALIZAR.FORM_INCOMPLETO');
    }
  }
  private showConfirmationDialog() {
    this.dialog
      .open(ConfirmDialog, {
        width: '300px',
        data: {
          title: 'M_FINALIZAR.FIN_LAVADO',
          text: 'M_FINALIZAR.CONFIRM_FINALIZAR_LAVADO',
          btn_text: false,
        },
        panelClass: 'dialog',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.finalizarLavado(this.lavadoFinalizado());
        }
      });
  }
  private finalizarLavado(lavado: LavadoVO) {
    this.subs.add(
      this.dataApi.finalizarLavado(lavado).subscribe({
        next: () => {
          this.notificationStore.success('M_FINALIZAR.FINALIZAR_LAVADO_OK');
          this.showStatusLavadoFinalizado();
        },
        error: () => {
          this.notificationStore.error('M_FINALIZAR.FINALIZAR_LAVADO_ERR');
        },
      }),
    );
  }
  private lavadoFinalizado(): LavadoVO {
    return {
      ...this.lavado,
      endUserId: this.form.value.operario,
      finishObservation: this.form.value.observaciones,
      status: LavadoStatus.FINISHED,
      endDate: new Date(),
      photos: this.photos, //FIXME: Por consultar
    };
  }
  private goToInicio() {
    this.navigation.goToInicio();
  }
  onVolverInicio() {
    this.form.markAsPristine();
    this.goToInicio();
  }
  private showStatusLavadoFinalizado() {
    this.titleStore.updateTitle('TITLES.LAVADO_FINALIZADO');
    this.form.disable();
    this.showLavadoFinalizado = true;
    this.form.markAsPristine();
    window.scrollTo(0, 0);
  }
}
