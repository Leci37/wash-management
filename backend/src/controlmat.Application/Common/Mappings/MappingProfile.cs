using AutoMapper;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Entities;

namespace Controlmat.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Washing, WashingResponseDto>()
            .ForMember(dest => dest.MachineName, opt => opt.MapFrom(src => src.Machine.Name))
            .ForMember(dest => dest.StartUserName, opt => opt.MapFrom(src => src.StartUser.UserName))
            .ForMember(dest => dest.EndUserName, opt => opt.MapFrom(src => src.EndUser != null ? src.EndUser.UserName : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.StatusDescription, opt => opt.MapFrom(src => src.Status == 'P' ? "En Progreso" : "Finalizado"));

        CreateMap<Washing, ActiveWashDto>()
            .ForMember(dest => dest.MachineName, opt => opt.MapFrom(src => src.Machine.Name))
            .ForMember(dest => dest.StartUserName, opt => opt.MapFrom(src => src.StartUser.UserName))
            .ForMember(dest => dest.ProtCount, opt => opt.MapFrom(src => src.Prots.Count))
            .ForMember(dest => dest.PhotoCount, opt => opt.MapFrom(src => src.Photos.Count));

        CreateMap<Prot, ProtDto>();
        CreateMap<ProtDto, Prot>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.WashingId, opt => opt.Ignore())
            .ForMember(dest => dest.Washing, opt => opt.Ignore());

        CreateMap<Photo, PhotoDto>()
            .ForMember(dest => dest.DownloadUrl, opt => opt.MapFrom(src => $"/api/photos/{src.Id}/download"));
        CreateMap<User, UserDto>();

        CreateMap<Machine, MachineDto>()
            .ForMember(dest => dest.IsAvailable, opt => opt.Ignore());
    }
}
