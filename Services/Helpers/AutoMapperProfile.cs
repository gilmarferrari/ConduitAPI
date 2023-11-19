using AutoMapper;
using Model;
using Model.Queries.Responses;

namespace Services.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserResponse>();
            CreateMap<Message, MessageResponse>()
                .ForMember(m => m.IsDeleted, opt => opt.MapFrom(src => src.Content.Equals(AppParameters.DeletedMessage)));
            CreateMap<ChatGroup, ChatGroupResponse>();
            CreateMap<ChatGroupMessage, ChatGroupMessageResponse>()
                .ForMember(m => m.IsDeleted, opt => opt.MapFrom(src => src.Content.Equals(AppParameters.DeletedMessage)));
            CreateMap<User, ChatUserResponse>()
                .ForMember(u => u.LastSeen, opt => opt.MapFrom((src, _, _, res) =>
                Extensions.GetLastSeen(src.LastSeen, ((List<int>)res.Items["ActiveClientsUserIDs"]).Contains(src.ID))));

            CreateMap<User, string>().ConvertUsing(u => u != null ? u.Name : null);
            CreateMap<Role, string>().ConvertUsing(r => r != null ? r.Code : null);
        }
    }
}
