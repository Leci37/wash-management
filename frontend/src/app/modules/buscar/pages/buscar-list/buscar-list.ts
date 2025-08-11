import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { TitleStore } from '../../../shell/title-store';
import { BuscarStore } from '../../buscar-store';

@Component({
  selector: 'cm-buscar-list',
  templateUrl: './buscar-list.html',
  styleUrl: './buscar-list.scss',
  imports: [CommonModule],
})
export class BuscarList implements OnInit {
  private titleStore = inject(TitleStore);
  private buscarStore = inject(BuscarStore);

  lavados$ = this.buscarStore.resultadoBusqueda$;

  ngOnInit() {
    this.titleStore.updateTitle('TITLES.BUSCAR_RES');
  }
}
