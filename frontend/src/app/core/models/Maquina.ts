export class MaquinaEstado {
  public static readonly disponible = true;
  public static readonly enUso = false;
}

export interface MaquinaDTO {
  maqId: number;
  maqName: string; //varchar(50)
}

export class MaquinaVO {
  public readonly maqId!: number;
  public readonly maqName!: string;
  public disponible?: boolean = true; //TOCHECK

  public static parseDTOtoVO(dto: MaquinaDTO): MaquinaVO {
    return {
      maqId: dto.maqId,
      maqName: dto.maqName,
    };
  }
  public static parseVOtoDTO(vo: MaquinaVO): MaquinaDTO {
    return {
      maqId: vo.maqId,
      maqName: vo.maqName,
    };
  }
}
