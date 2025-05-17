using AutoMapper;
using Conduit.Application.Queries.Responses;
using Conduit.Domain.Entities;

namespace Conduit.Application.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserResponse>();
            CreateMap<UserAccount, UserAccountResponse>();
            CreateMap<Message, MessageResponse>();
            CreateMap<ChatGroup, ChatGroupResponse>();
            CreateMap<ChatGroupMessage, ChatGroupMessageResponse>();
            CreateMap<ChatGroupParticipant, ChatGroupParticipantResponse>()
                .ForMember(p => p.User, opt => opt.MapFrom(src => src.Participant));
            CreateMap<User, ChatUserResponse>()
                .ForMember(u => u.LastSeen, opt => opt.MapFrom((src, _, _, res) =>
                Extensions.GetLastSeen(src.LastSeen, res.Items.ContainsKey("ActiveClientsUserIDs") ? ((List<Guid>)res.Items["ActiveClientsUserIDs"]).Contains(src.ID) : false)));

            CreateMap<ChatGroupRule, int>().ConvertUsing(x => x != null ? x.Rule : default);
            CreateMap<User, string>().ConvertUsing(x => x != null ? x.Name : null);
            CreateMap<UserRole, string>().ConvertUsing(x => x != null ? ((RoleList)x.Role).ToString() : null);
        }
    }
}
