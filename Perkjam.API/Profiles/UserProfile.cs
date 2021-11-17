using AutoMapper;

namespace Perkjam.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // map from User (entity) to User, and back
            CreateMap<Entities.User, Model.User>().ReverseMap();
            
            // map from UserForCreation to User
            // Ignore properties that shouldn't be mapped
            CreateMap<Model.UserForCreation, Entities.User>();
            
            // map from UserForUpdate to User
            // Ignore properties that shouldn't be mapped
            CreateMap<Model.UserForUpdate, Entities.User>();

        }
    }
}