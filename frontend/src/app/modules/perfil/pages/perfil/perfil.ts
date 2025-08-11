import { Component, inject, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { NavigationStore } from '../../../shell/navigation-store';
import { NavigationRoutes } from '../../../../app.routes';
import { TitleStore } from '../../../shell/title-store';
import { PerfilStore, Profiles } from '../../../../core/services/perfil-store';

@Component({
  selector: 'cm-perfil',
  imports: [TranslateModule],
  templateUrl: './perfil.html',
  styleUrl: './perfil.scss',
})
export class Perfil implements OnInit {
  private titleStore = inject(TitleStore);
  private navigation = inject(NavigationStore);
  private perfilStore = inject(PerfilStore);

  get routes() {
    return NavigationRoutes;
  }
  protected profiles = Profiles;
  ngOnInit() {
    this.titleStore.updateTitle('Perfil');
  }
  goTo(route: string) {
    this.navigation.goTo(route);
  }
  onChangeProfile(profile: Profiles) {
    this.perfilStore.setPerfil(profile);
    this.navigation.goToInicio();
  }
}
