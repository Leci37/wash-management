using System;

namespace controlmat.Domain.Entities;

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Role { get; set; } = "WarehouseUser";
}

