import { PhotoDTO, PhotoVO } from './Photo';

export enum LavadoStatus {
  IN_PROGRESS = 'P',
  FINISHED = 'F',
}
export interface LavadoDTO {
  washingId: number; // long: YYMMDDXX (correlativo de dos dígitos)
  machine: number; // integer
  startDate: Date; // DateTime
  endDate: Date | null; // DateTime
  startUserId: number; // integer
  endUserId: number | null; // integer
  status: string; // varchar: P (en progreso) / F (finalizado)
  protId: string; // varchar(7): PROTXXX
  loteNumber: string; // varchar(4): NLXX
  bagNumber: string; // varchar(5): XX/XX
  startObservation: string; // varchar(100)
  finishObservation: string; // varchar(100)
  prots?: string[]; //FIXME Por consultar
  photos?: PhotoDTO[]; //FIXME Por consultar
}

export class LavadoVO {
  public readonly washingId!: number;
  public readonly machine!: number;
  public readonly startDate!: Date;
  public readonly endDate!: Date | null;
  public readonly startUserId!: number;
  public readonly endUserId!: number | null;
  public readonly status!: string;
  public readonly protId!: string;
  public readonly loteNumber!: string;
  public readonly bagNumber!: string;
  public readonly startObservation!: string;
  public readonly finishObservation!: string;
  public readonly prots?: string[];
  public readonly photos?: PhotoVO[];

  public static parseDTOtoVO(dto: LavadoDTO): LavadoVO {
    return {
      washingId: dto.washingId,
      machine: dto.machine,
      startDate: dto.startDate,
      endDate: dto.endDate,
      startUserId: dto.startUserId,
      endUserId: dto.endUserId,
      status: dto.status,
      protId: dto.protId,
      loteNumber: dto.loteNumber,
      bagNumber: dto.bagNumber,
      startObservation: dto.startObservation,
      finishObservation: dto.finishObservation,
      prots: dto.prots ? [...dto.prots] : undefined,
      photos: dto.photos ? [...dto.photos] : undefined,
    };
  }

  public static parseVOtoDTO(vo: LavadoVO): LavadoDTO {
    return {
      washingId: vo.washingId,
      machine: vo.machine,
      startDate: vo.startDate,
      endDate: vo.endDate,
      startUserId: vo.startUserId,
      endUserId: vo.endUserId,
      status: vo.status,
      protId: vo.protId,
      loteNumber: vo.loteNumber,
      bagNumber: vo.bagNumber,
      startObservation: vo.startObservation,
      finishObservation: vo.finishObservation,
      prots: vo.prots ? [...vo.prots] : undefined,
      photos: vo.photos ? [...vo.photos] : undefined,
    };
  }
}
