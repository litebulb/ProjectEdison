using System;
using AutoMapper;
using Edison.Core.Common.Models;
using Edison.Common.DAO;

namespace Edison.Api.AutoMapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Event Clusters
            CreateMap<EventClusterDAO, EventClusterModel>()
                .ForMember(dto => dto.EventClusterId, opts => opts.MapFrom(src => src.Id));
            CreateMap<DeviceDAO, EventClusterDeviceDAOObject>()
               .ForMember(dto => dto.DeviceId, opts => opts.MapFrom(src => src.Id))
               .ForMember(dto => dto.DeviceType, opts => opts.MapFrom(src => src.DeviceType));

            //Devices
            CreateMap<DeviceDAO, DeviceModel>()
                .ForMember(dto => dto.DeviceId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dto => dto.LastAccessTime, opts => opts.MapFrom(src => src.LastAccessTime))
                .ForMember(dto => dto.Online, opts => opts.MapFrom(src => src.LastAccessTime > DateTime.UtcNow.AddMinutes(-15)));
            CreateMap<DeviceDAO, DeviceMapModel>()
                .ForMember(dto => dto.DeviceId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dto => dto.Online, opts => opts.MapFrom(src => src.LastAccessTime > DateTime.UtcNow.AddMinutes(-15)));
            CreateMap<DeviceTwinModel, DeviceDAO>()
                .ForMember(dto => dto.Id, opts => opts.MapFrom(src => src.DeviceId))
                .ForMember(dto => dto.DeviceType, opts => opts.MapFrom(src => src.Tags.DeviceType))
                .ForMember(dto => dto.SSID, opts => opts.MapFrom(src => src.Tags.SSID))
                .ForMember(dto => dto.Sensor, opts => opts.MapFrom(src => src.Tags.Sensor))
                .ForMember(dto => dto.Name, opts => opts.MapFrom(src => src.Tags.Name))
                .ForMember(dto => dto.Location1, opts => opts.MapFrom(src => src.Tags.Location1))
                .ForMember(dto => dto.Location2, opts => opts.MapFrom(src => src.Tags.Location2))
                .ForMember(dto => dto.Location3, opts => opts.MapFrom(src => src.Tags.Location3))
                .ForMember(dto => dto.Geolocation, opts => opts.MapFrom(src => src.Tags.Geolocation))
                .ForMember(dto => dto.Custom, opts => opts.MapFrom(src => src.Tags.Custom))
                .ForMember(dto => dto.Desired, opts => opts.MapFrom(src => src.Properties.Desired))
                .ForMember(dto => dto.Reported, opts => opts.MapFrom(src => src.Properties.Reported));
            CreateMap<DeviceMobileModel, DeviceDAO>()
                .ForMember(dto => dto.Id, opts => opts.MapFrom(src => src.DeviceId));
            CreateMap<DeviceDAO, DeviceMobileModel>()
                .ForMember(dto => dto.DeviceId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dto => dto.RegistrationId, opts => opts.Ignore())
                .ForMember(dto => dto.Platform, opts => opts.Ignore())
                .ForMember(dto => dto.MobileId, opts => opts.Ignore())
                .ForMember(dto => dto.Email, opts => opts.Ignore());

            //Response
            CreateMap<ActionPlanDAO, ActionPlanModel>()
                .ForMember(dto => dto.ActionPlanId, opts => opts.MapFrom(src => src.Id));
            CreateMap<ActionPlanDAO, ActionPlanListModel>()
                .ForMember(dto => dto.ActionPlanId, opts => opts.MapFrom(src => src.Id));
            CreateMap<ResponseDAO, ResponseModel>()
                .ForMember(dto => dto.ResponseId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dto => dto.Name, opts => opts.MapFrom(src => src.ActionPlan.Name))
                .ForMember(dto => dto.Icon, opts => opts.MapFrom(src => src.ActionPlan.Icon))
                .ForMember(dto => dto.Color, opts => opts.MapFrom(src => src.ActionPlan.Color))
                .ForMember(dto => dto.StartDate, opts => opts.MapFrom(src => src.CreationDate))
                .ForMember(dto => dto.EndDate, opts => opts.MapFrom(src => src.EndDate))
                .ForMember(dto => dto.ActionPlan, opts => opts.MapFrom(src => src.ActionPlan));
                
            CreateMap<ResponseDAO, ResponseLightModel>()
                .ForMember(dto => dto.ResponseId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dto => dto.StartDate, opts => opts.MapFrom(src => src.CreationDate))
                .ForMember(dto => dto.Name, opts => opts.MapFrom(src => src.ActionPlan.Name))
                .ForMember(dto => dto.Icon, opts => opts.MapFrom(src => src.ActionPlan.Icon))
                .ForMember(dto => dto.Color, opts => opts.MapFrom(src => src.ActionPlan.Color))
                .ForMember(dto => dto.AcceptSafeStatus, opts => opts.MapFrom(src => src.ActionPlan.AcceptSafeStatus))
                .ForMember(dto => dto.EndDate, opts => opts.MapFrom(src => src.EndDate))
                .ForMember(dto => dto.IsActive, opts => opts.Condition(dao => dao.EndDate < DateTime.UtcNow));
            CreateMap<ResponseActionPlanDAOObject, ResponseActionPlanModel>();
            CreateMap<ResponseActionDAOObject, ResponseActionModel>();
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

            //Notifications
            CreateMap<NotificationDAO, NotificationModel>()
                .ForMember(dto => dto.NotificationId, opts => opts.MapFrom(src => src.Id));

        }
    }
}
