import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { Component, inject, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { MatButtonModule } from '@angular/material/button';

import { TitleStore } from '../../../shell/title-store';
import { NavigationRoutes } from '../../../../app.routes';
import { DataApi } from '../../../../core/services/data-api';
import { NavigationStore } from '../../../shell/navigation-store';
import { MaquinasStore } from '../../../../core/services/maquinas-store';
import { PerfilStore, Profiles } from '../../../../core/services/perfil-store';

@Component({
  selector: 'cm-inicio',
  imports: [CommonModule, MatButtonModule, TranslateModule],
  templateUrl: './inicio.html',
  styleUrl: './inicio.scss',
})
export class Inicio implements OnInit {
  private titleStore = inject(TitleStore);
  private navigation = inject(NavigationStore);
  private perfilStore = inject(PerfilStore);
  private dataApi = inject(DataApi);
  private maquinasStore = inject(MaquinasStore);
  private subs = new Subscription();

  protected maquina1 = this.maquinasStore.maquina1;
  protected maquina2 = this.maquinasStore.maquina2;
  buscarHidden = false;

  ngOnInit() {
    this.titleStore.updateTitle('TITLES.INICIO');
    this.adminProfileChanges();
    this.getEstadoMaquinas();
  }
  get navigationRoutes() {
    return NavigationRoutes;
  }

  goTo(route: string) {
    this.navigation.goTo(route);
  }
  getEstadoMaquinas() {
    this.subs.add(
      this.dataApi.getMaquinas().subscribe((maquinas) => {
        maquinas.forEach((maq) => {
          this.maquinasStore.setMaquina(maq.maqId, maq.disponible!);
        });
      }),
    );
  }
  private adminProfileChanges() {
    this.subs.add(
      this.perfilStore.perfil$.subscribe((perfil) => {
        this.buscarHidden = perfil !== Profiles.Admin;
      }),
    );
  }
}
