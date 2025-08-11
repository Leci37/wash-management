import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { provideNativeDateAdapter } from '@angular/material/core';

import { DataApi } from '../../../../core/services/data-api';
import { TitleStore } from '../../../shell/title-store';
import { NotificationStore } from '../../../../core/services/notification-store';
import { NavigationStore } from '../../../shell/navigation-store';
import { NavigationRoutes } from '../../../../app.routes';
import { BuscarStore } from '../../buscar-store';
import { FormDirty } from '../../../../core/guard/unsavedChanges';
import { BusqLavadosFilterVO } from '../../../../core/models/BusquedaLavadosFilter';
import { LavadoVO } from '../../../../core/models/Lavado';

@Component({
  selector: 'cm-buscar',
  templateUrl: './buscar.html',
  styleUrl: './buscar.scss',
  imports: [
    CommonModule,
    TranslateModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatIconModule,
    MatDatepickerModule,
  ],
  providers: [
    provideNativeDateAdapter(),
    // { provide: MAT_DATE_FORMATS, useValue: MY_DATE_FORMATS },
    // DatePipe,
  ],
})
export class Buscar implements OnInit, OnDestroy, FormDirty {
  private titleStore = inject(TitleStore);
  private dataApi = inject(DataApi);
  private fb = inject(FormBuilder);
  private navigation = inject(NavigationStore);
  private notificationStore = inject(NotificationStore);
  private buscarStore = inject(BuscarStore);

  subs: Subscription = new Subscription();
  operarios$ = this.dataApi.getOperarios();
  prots$ = this.dataApi.getProts();
  form: FormGroup;
  lavados: LavadoVO[] = [];

  ngOnInit() {
    this.titleStore.updateTitle('TITLES.BUSCAR_LAVADO');
  }
  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }
  constructor() {
    this.form = this.fb.group({
      prots: [[]],
      operarios: [[]],
      fechaDesde: [new Date()],
      fechaHasta: [new Date()],
    });
  }
  isFormDirty(): boolean {
    return this.form.dirty;
  }
  onBuscar() {
    if (this.form.valid) {
      this.dataApi.searchLavados(this.filter()).subscribe({
        next: (res) => {
          this.buscarStore.setResultadoBusqueda(res);
          this.navigation.goTo(NavigationRoutes.Buscar_List);
        },
        error: (err) => {
          this.notificationStore.error('_Error al buscar lavados');
          console.error('Error al buscar lavados:', err);
        },
      });
    }
  }
  private filter(): BusqLavadosFilterVO {
    return {
      prots: this.form.get('prots')?.value,
      operarios: this.form.get('operarios')?.value,
      fechaDesde: this.form.get('fechaDesde')?.value || null,
      fechaHasta: this.form.get('fechaHasta')?.value || null,
    };
  }
}
