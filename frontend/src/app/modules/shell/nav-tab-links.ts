export interface NavTabLink {
  label: string;
  icon: string;
  activeIcon: string;
  path: string;
}

export const nav_tab_links: NavTabLink[] = [
  // { label: 'Buscar', path: '/nuevo' },
  {
    label: 'NAV_TAB.INICIO',
    icon: 'home',
    activeIcon: 'home-active',
    path: 'inicio',
  },
  {
    label: 'NAV_TAB.BUSCAR',
    icon: 'buscar',
    activeIcon: 'buscar-yellow',
    path: 'buscar',
  },
  {
    label: 'NAV_TAB.NUEVO',
    icon: 'play_arrow',
    activeIcon: 'play_arrow-active',
    path: 'nuevo',
  },
  {
    label: 'NAV_TAB.FINALIZAR',
    icon: 'pause',
    activeIcon: 'pause-active',
    path: 'finalizar',
  },
  {
    label: 'NAV_TAB.PERFIL',
    icon: 'user',
    activeIcon: 'user-active',
    path: 'perfil',
  },
];
