export interface UserDTO {
  userId: number;
  userName: string; //varchar(100)
}

export class UserVO {
  public readonly userId!: number;
  public readonly userName!: string;

  public static parseDTOtoVO(dto: UserDTO): UserVO {
    return {
      userId: dto.userId,
      userName: dto.userName,
    };
  }
  public static parseVOtoDTO(vo: UserVO): UserDTO {
    return {
      userId: vo.userId,
      userName: vo.userName,
    };
  }
}
