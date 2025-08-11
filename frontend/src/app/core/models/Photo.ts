export interface PhotoDTO {
  name: number;
  photo: string | ArrayBuffer | Blob;
}

export class PhotoVO {
  public readonly name!: number;
  public readonly photo!: string | ArrayBuffer | Blob;

  public static parseDTOtoVO(dto: PhotoDTO): PhotoVO {
    return {
      name: dto.name,
      photo: dto.photo,
    };
  }
  public static parseVOtoDTO(vo: PhotoVO): PhotoDTO {
    return {
      name: vo.name,
      photo: vo.photo,
    };
  }
}
