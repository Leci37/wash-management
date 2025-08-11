import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export enum Profiles {
  Admin = 'Admin',
  NormalUser = 'NormalUser',
}
@Injectable({
  providedIn: 'root',
})
export class PerfilStore {
  private readonly _perfil = new BehaviorSubject<Profiles>(Profiles.NormalUser);
  public readonly perfil$ = this._perfil.asObservable();

  public setPerfil(perfil: Profiles) {
    this._perfil.next(perfil);
  }
}
