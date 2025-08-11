using AutoMapper;
using Controlmat.Domain.Entities;
using Controlmat.Application.Common.Dto;

namespace Controlmat.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<Washing, WashingResponseDto>()
                .ForMember(dest => dest.MachineName, opt => opt.MapFrom(src => src.Machine.Name))
                .ForMember(dest => dest.StartUserName, opt => opt.MapFrom(src => src.StartUser.UserName))
                .ForMember(dest => dest.EndUserName, opt => opt.MapFrom(src => src.EndUser != null ? src.EndUser.UserName : null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Prot, ProtDto>().ReverseMap();
            CreateMap<Photo, PhotoDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Machine, MachineDto>().ReverseMap();
        }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class MachineDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
