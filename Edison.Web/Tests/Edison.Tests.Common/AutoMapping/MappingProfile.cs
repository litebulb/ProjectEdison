using AutoMapper;
using Edison.Common.DAO;
using Edison.Core.Common.Models;

namespace Edison.Tests
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Event Clusters
            CreateMap<EventClusterDAO, EventClusterModel>()
                .ForMember(dto => dto.EventClusterId, opts => opts.MapFrom(src => src.Id));
            CreateMap<DeviceDAO, EventClusterDAODevice>()
               .ForMember(dto => dto.DeviceId, opts => opts.MapFrom(src => src.Id));

            //Devices
            CreateMap<DeviceDAO, DeviceModel>()
                .ForMember(dto => dto.DeviceId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dto => dto.Desired, opts => opts.MapFrom(src => src.Desired));

            //Users
            CreateMap<UserDAO, UserModel>()
                .ForMember(dto => dto.UserId, opts => opts.MapFrom(src => src.Id));
        }
    }
}
