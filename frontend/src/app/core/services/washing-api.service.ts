import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ProtDto {
  protId: string;
  batchNumber: string;
  bagNumber: string;
}

export interface NewWashDto {
  machineId: number;
  startUserId: number;
  startObservation?: string;
  protEntries: ProtDto[];
}

export interface WashingResponseDto {
  washingId: number;
  machineId: number;
  machineName: string;
  startUserId: number;
  startUserName: string;
  endUserId?: number;
  endUserName?: string;
  startDate: string;
  endDate?: string;
  status: string;
  startObservation?: string;
  finishObservation?: string;
  prots: ProtDto[];
}

@Injectable({ providedIn: 'root' })
export class WashingApiService {
  private baseUrl = 'http://localhost:5274/api/washing';

  constructor(private http: HttpClient) {}

  startWash(dto: NewWashDto): Observable<WashingResponseDto> {
    return this.http.post<WashingResponseDto>(this.baseUrl, dto);
    }

  getActiveWashes(): Observable<WashingResponseDto[]> {
    return this.http.get<WashingResponseDto[]>(`${this.baseUrl}/active`);
  }
}
