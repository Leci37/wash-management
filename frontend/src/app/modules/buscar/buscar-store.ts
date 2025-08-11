import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { LavadoVO } from '../../core/models/Lavado';

@Injectable({
  providedIn: 'root',
})
export class BuscarStore {
  private readonly _resultadoBusqueda = new BehaviorSubject<LavadoVO[]>([]);
  public readonly resultadoBusqueda$ = this._resultadoBusqueda.asObservable();

  public setResultadoBusqueda(res: LavadoVO[]) {
    this._resultadoBusqueda.next(res);
  }
}
