using AutoMapper;
using Servora.Application.Catalog.CreateMenuItem;
using Servora.Domain.Models;

namespace Servora.Application.Mappings;

public sealed class MenuItemMappingProfile : Profile
{
    public MenuItemMappingProfile()
    {
        CreateMap<CreateMenuItemCommand, MenuItem>()
            .ForMember(item => item.Name, options => options.MapFrom(command => command.Name.Trim()))
            .ForMember(item => item.TenantId, options => options.Ignore());
    }
}
