import { Component, inject, OnInit } from '@angular/core';
import { NavigationStore } from '../../../shell/navigation-store';
import { TranslateModule } from '@ngx-translate/core';
import { NavigationRoutes } from '../../../../app.routes';
import { TitleStore } from '../../../shell/title-store';
import { MaquinasStore } from '../../../../core/services/maquinas-store';

@Component({
  selector: 'cm-finalizar',
  imports: [TranslateModule],
  templateUrl: './finalizar-menu.html',
  styleUrl: './finalizar-menu.scss',
})
export class FinalizarMenu implements OnInit {
  private titleStore = inject(TitleStore);
  private navigation = inject(NavigationStore);
  private maquinasStore = inject(MaquinasStore);

  protected maquina1 = this.maquinasStore.maquina1;
  protected maquina2 = this.maquinasStore.maquina2;

  get routes() {
    return NavigationRoutes;
  }
  ngOnInit() {
    this.titleStore.updateTitle('Finalizar lavado');
  }
  goTo(route: string) {
    this.navigation.goTo(route);
  }
}
