import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { UserDTO, UserVO } from '../models/User';
import { LavadoDTO, LavadoStatus, LavadoVO } from '../models/Lavado';
import { MaquinaVO } from '../models/Maquina';
import { BusqLavadosFilterVO } from '../models/BusquedaLavadosFilter';

@Injectable({
  providedIn: 'root',
})
export class DataApi {
  getOperarios(): Observable<UserVO[]> {
    return of(operariosMock.map(UserVO.parseDTOtoVO));
  }
  getMaquinas(): Observable<MaquinaVO[]> {
    return of(maquinasMock);
  }
  getProts(): Observable<string[]> {
    return of(protsMock);
  }
  setMaquinaOcupada(maqId: number) {
    const maquina = maquinasMock.find((m) => m.maqId === maqId);
    if (maquina) {
      maquina.disponible = false;
    }
  }
  setMaquinaDisponible(maqId: number) {
    const maquina = maquinasMock.find((m) => m.maqId === maqId);
    if (maquina) {
      maquina.disponible = true;
    }
  }
  iniciarLavado(lavado: LavadoVO): Observable<LavadoVO> {
    console.log('Iniciar lavado con datos:', lavado);
    lavadosMock.push(LavadoVO.parseVOtoDTO(lavado));
    this.setMaquinaOcupada(lavado.machine);
    return of(LavadoVO.parseDTOtoVO(lavadosMock[lavadosMock.length - 1]));
  }
  finalizarLavado(lavado: LavadoVO): Observable<LavadoVO> {
    console.log('Finalizar lavado con datos:', lavado);
    const index = lavadosMock.findIndex(
      (l) => l.washingId === lavado.washingId,
    );
    if (index !== -1) {
      lavadosMock[index] = LavadoVO.parseVOtoDTO(lavado);
      this.setMaquinaDisponible(lavado.machine);
    }
    return of(lavadosMock[index]);
  }
  searchLavados(filter: BusqLavadosFilterVO): Observable<LavadoVO[]> {
    console.log('Buscar lavados con filtro:', filter);
    // Implement search logic here
    return of(lavadosMock.map(LavadoVO.parseDTOtoVO));
  }
  getLavadoEnCurso(maqId: number | string): Observable<LavadoVO | null> {
    const maqIdNum = typeof maqId === 'string' ? parseInt(maqId, 10) : maqId;
    const lavado = lavadosMock.find(
      (lav) =>
        lav.machine === maqIdNum && lav.status === LavadoStatus.IN_PROGRESS,
    );
    return of(lavado ? LavadoVO.parseDTOtoVO(lavado) : null);
  }
}
// Mock data -----------------------------------------------
const maquinasMock: MaquinaVO[] = [
  { maqId: 1, maqName: 'Máquina 1', disponible: false },
  { maqId: 2, maqName: 'Máquina 2', disponible: true },
];
const operariosMock: UserDTO[] = [
  {
    userId: 1,
    userName: 'Operario 1',
  },
  {
    userId: 2,
    userName: 'Operario 2',
  },
  {
    userId: 3,
    userName: 'Operario 3',
  },
];
const protsMock: string[] = ['Prot 1', 'Prot 2', 'Prot 3'];

const lavadosMock: LavadoDTO[] = [
  {
    washingId: 1,
    machine: 1,
    startDate: new Date(),
    endDate: null,
    startUserId: 1,
    endUserId: null,
    status: LavadoStatus.IN_PROGRESS,
    protId: 'PROT001',
    loteNumber: 'NL01',
    bagNumber: '01/02',
    startObservation: 'Inicio lavado',
    finishObservation: '',
    prots: ['Prot 1', 'Prot 2'],
  },
  {
    washingId: 2,
    machine: 2,
    startDate: new Date('2025-07-11T11:00:00'),
    endDate: new Date('2025-07-11T14:00:00'),
    startUserId: 2,
    endUserId: null,
    status: LavadoStatus.FINISHED,
    protId: 'PROT002',
    loteNumber: 'NL02',
    bagNumber: '02/03',
    startObservation: 'Inicio lavado',
    finishObservation: 'Lavado completado sin incidencias',
    prots: ['Prot 2', 'Prot 3'],
  },
];
