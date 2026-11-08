using AutoMapper;
using Servora.Application.Users.CreateUser;
using Servora.Domain.Models;

namespace Servora.Application.Mappings;

public sealed class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<CreateUserCommand, User>()
            .ForMember(user => user.Name, options => options.MapFrom(command => command.Name.Trim()))
            .ForMember(user => user.Email, options => options.MapFrom(command => command.Email.Trim()))
            .ForMember(user => user.UserName, options => options.MapFrom(command => command.Email.Trim()))
            .ForMember(user => user.TenantId, options => options.Ignore());
    }
}
