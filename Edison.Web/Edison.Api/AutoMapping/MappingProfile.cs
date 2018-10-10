using AutoMapper;
using Edison.Common.DAO;
using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Api.AutoMapping
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
                .ForMember(dto => dto.Online, opts => opts.MapFrom(src => src.LastAccessTime > DateTime.UtcNow.AddMinutes(-15)));
            CreateMap<DeviceDAO, DeviceMapModel>()
                .ForMember(dto => dto.DeviceId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dto => dto.Online, opts => opts.MapFrom(src => src.LastAccessTime > DateTime.UtcNow.AddMinutes(-15)));
            CreateMap<DeviceTwinModel, DeviceDAO>()
                .ForMember(dto => dto.Id, opts => opts.MapFrom(src => src.DeviceId))
                .ForMember(dto => dto.DeviceType, opts => opts.MapFrom(src => src.Tags.DeviceType))
                .ForMember(dto => dto.Sensor, opts => opts.MapFrom(src => src.Tags.Sensor))
                .ForMember(dto => dto.LocationName, opts => opts.MapFrom(src => src.Tags.LocationName))
                .ForMember(dto => dto.LocationLevel1, opts => opts.MapFrom(src => src.Tags.LocationLevel1))
                .ForMember(dto => dto.LocationLevel2, opts => opts.MapFrom(src => src.Tags.LocationLevel2))
                .ForMember(dto => dto.LocationLevel3, opts => opts.MapFrom(src => src.Tags.LocationLevel3))
                .ForMember(dto => dto.Geolocation, opts => opts.MapFrom(src => src.Tags.Geolocation))
                .ForMember(dto => dto.Custom, opts => opts.MapFrom(src => src.Tags.Custom))
                .ForMember(dto => dto.Desired, opts => opts.MapFrom(src => src.Properties.Desired))
                .ForMember(dto => dto.Reported, opts => opts.MapFrom(src => src.Properties.Reported));

            //Response
            CreateMap<ActionPlanDAO, ActionPlanModel>()
                .ForMember(dto => dto.ActionPlanId, opts => opts.MapFrom(src => src.Id));
            CreateMap<ActionPlanDAO, ActionPlanListModel>()
                .ForMember(dto => dto.ActionPlanId, opts => opts.MapFrom(src => src.Id));
            CreateMap<ResponseDAO, ResponseModel>()
                .ForMember(dto => dto.ResponseId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dto => dto.StartDate, opts => opts.MapFrom(src => src.CreationDate))
                .ForMember(dto => dto.EndDate, opts => opts.MapFrom(src => src.EndDate));
            CreateMap<ResponseDAO, ResponseLightModel>()
                .ForMember(dto => dto.ResponseId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dto => dto.StartDate, opts => opts.MapFrom(src => src.CreationDate))
                .ForMember(dto => dto.Name, opts => opts.MapFrom(src => src.ActionPlan.Name))
                .ForMember(dto => dto.Icon, opts => opts.MapFrom(src => src.ActionPlan.Icon))
                .ForMember(dto => dto.Color, opts => opts.MapFrom(src => src.ActionPlan.Color))
                .ForMember(dto => dto.EndDate, opts => opts.MapFrom(src => src.EndDate));
            CreateMap<ResponseDAO, ResponseUpdateModel>()
                .ForMember(dto => dto.ResponseId, opts => opts.MapFrom(src => src.Id));
            CreateMap<ActionPlanUpdateModel, ActionPlanDAO>()
                .ForMember(dto => dto.Id, opts => opts.MapFrom(src => src.ActionPlanId))
                .ForMember(dto => dto.CreationDate, opts => opts.Ignore())
                .ForMember(dto => dto.UpdateDate, opts => opts.Ignore())
                .ForMember(dto => dto.ETag, opts => opts.Ignore())
                .ForMember(dto => dto.CreationDate, opts => opts.Ignore());
            CreateMap<ActionPlanCreationModel, ActionPlanDAO>()
                .ForMember(dto => dto.CreationDate, opts => opts.Ignore())
                .ForMember(dto => dto.UpdateDate, opts => opts.Ignore())
                .ForMember(dto => dto.ETag, opts => opts.Ignore())
                .ForMember(dto => dto.Id, opts => opts.Ignore());

            //Users
            CreateMap<UserDAO, UserModel>()
                .ForMember(dto => dto.UserId, opts => opts.MapFrom(src => src.Id));

            //Notifications
            CreateMap<NotificationDAO, NotificationModel>()
                .ForMember(dto => dto.NotificationId, opts => opts.MapFrom(src => src.Id));

        }
    }
}
