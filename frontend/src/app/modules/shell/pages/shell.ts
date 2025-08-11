import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  OnDestroy,
  OnInit,
  signal,
  Signal,
} from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';
import { filter, Subscription } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';

import { CustomIcon, customIcons } from '../custom-icons';
import { nav_tab_links, NavTabLink } from '../nav-tab-links';
import { TitleStore } from '../title-store';
import { Languages } from '../../../core/languages/languages';
import { PerfilStore, Profiles } from '../../../core/services/perfil-store';
import { NavigationStore } from '../navigation-store';
import { NavigationRoutes } from '../../../app.routes';

@Component({
  selector: 'cm-shell',
  imports: [
    CommonModule,
    RouterOutlet,
    MatTabsModule,
    MatIconModule,
    TranslateModule,
  ],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell implements OnInit, OnDestroy {
  private router = inject(Router);
  private navigation = inject(NavigationStore);
  private perfilStore = inject(PerfilStore);

  private matIconRegistry = inject(MatIconRegistry);
  private domSanitizer = inject(DomSanitizer);
  private titleStore = inject(TitleStore);
  private translate = inject(TranslateService);

  private subs = new Subscription();
  // perfil = this.perfilStore.perfil;
  title: Signal<string> = this.titleStore.title;

  protected links: NavTabLink[] = [];
  activeLink = signal('');

  // constructor() {}
  ngOnInit() {
    this.createCustomIcons();
    this.translate.setDefaultLang(Languages.ES);
    this.translate.use(Languages.ES);
    this.adminProfileChanges();
    this.listenRouteChangesForNavbar();
    this.goToInicio(); //TODO cambiar a LOGIN cuando se implemente
  }
  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }
  private createCustomIcons() {
    customIcons.forEach((icon: CustomIcon) => {
      this.matIconRegistry.addSvgIcon(
        icon.svg,
        this.domSanitizer.bypassSecurityTrustResourceUrl(icon.path),
      );
    });
  }

  private listenRouteChangesForNavbar() {
    this.subs.add(
      this.router.events
        .pipe(filter((event) => event instanceof NavigationEnd))
        .subscribe((event: NavigationEnd) => {
          this.activeLink.set(this.extractPath(event.urlAfterRedirects));
        }),
    );
  }
  private extractPath(url: string): string {
    const segments = url.split('/');
    const path = segments.length > 1 ? segments[1] : '';
    return `${path}`;
  }
  private adminProfileChanges() {
    //FIXME cambiar cuando se implemente el login y auth
    this.subs.add(
      this.perfilStore.perfil$.subscribe((perfil) => {
        if (perfil === Profiles.Admin) {
          this.links = nav_tab_links;
        } else {
          this.links = nav_tab_links.filter(
            (link) => link.path !== NavigationRoutes.Buscar,
          );
        }
        console.log(`Profile changed to: ${perfil}`);
      }),
    );
  }
  onGoTo(link: NavTabLink) {
    this.navigation.goTo(link.path);
  }
  goToInicio() {
    this.navigation.goToInicio();
  }
}
