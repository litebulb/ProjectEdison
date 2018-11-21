using AutoMapper;
using Edison.Common.DAO;
using Edison.Core.Common.Models;
using Microsoft.Bot.Schema;

namespace Edison.ChatService
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ConversationReference, ChatUserSessionDAO>()
                .ForMember(dto => dto.Id, opts => opts.MapFrom(src => src.User.Id))
                .ForMember(dto => dto.Name, opts => opts.MapFrom(src => src.User.Name))
                .ForMember(dto => dto.Role, opts => opts.MapFrom(src => src.User.Role))
                .ForMember(dto => dto.BotId, opts => opts.MapFrom(src => src.Bot.Id))
                .ForMember(dto => dto.BotName, opts => opts.MapFrom(src => src.Bot.Name))
                .ForMember(dto => dto.ConversationId, opts => opts.MapFrom(src => src.Conversation.Id))
                .ForMember(dto => dto.ETag, opts => opts.Ignore())
                .ForMember(dto => dto.CreationDate, opts => opts.Ignore())
                .ForMember(dto => dto.UpdateDate, opts => opts.Ignore())
                .ForMember(dto => dto.UsersReadStatus, opts => opts.Ignore());
            CreateMap<ChatUserSessionDAO, ConversationReference>()
               .ForMember(dto => dto.ActivityId, opts => opts.Ignore())
               .ForMember(dto => dto.Conversation, opts => opts.MapFrom(src => new ConversationAccount(null, null, src.ConversationId, null, null)))
               .ForMember(dto => dto.User, opts => opts.MapFrom(src => new ChannelAccount(src.Id, src.Name, src.Role)))
               .ForMember(dto => dto.Bot, opts => opts.MapFrom(src => new ChannelAccount(src.BotId, src.BotName, null)));

            //Conversation
            CreateMap<ChatReportDAO, ChatReportModel>()
                .ForMember(dto => dto.ReportId, opts => opts.MapFrom(src => src.Id));
        }
    }
}
