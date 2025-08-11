import { Injectable, signal } from '@angular/core';
import { MaquinaEstado } from '../models/Maquina';

@Injectable({
  providedIn: 'root',
})
export class MaquinasStore {
  private _maquina_1 = signal(MaquinaEstado.disponible);
  private _maquina_2 = signal(MaquinaEstado.disponible);
  maquina1 = this._maquina_1.asReadonly();
  maquina2 = this._maquina_2.asReadonly();

  public setMaquina(maqId: number, estado: boolean) {
    if (maqId === 1) {
      this._maquina_1.set(estado);
    } else if (maqId === 2) {
      this._maquina_2.set(estado);
    }
  }
}
