export interface BusqLavadosFilterDTO {
  prots: string[];
  operarios: number[];
  fechaDesde: number | null;
  fechaHasta: number | null;
}

export class BusqLavadosFilterVO {
  public readonly prots!: string[];
  public readonly operarios!: number[];
  public readonly fechaDesde!: Date | null;
  public readonly fechaHasta!: Date | null;

  public static parseVOtoDTO(vo: BusqLavadosFilterVO): BusqLavadosFilterDTO {
    return {
      prots: vo.prots,
      operarios: vo.operarios,
      fechaDesde: vo.fechaDesde?.valueOf() || null,
      fechaHasta: vo.fechaHasta?.valueOf() || null,
    };
  }
}
