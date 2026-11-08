using AutoMapper;
using Servora.Application.Ordering.OpenTableSession;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Mappings;

public sealed class TableSessionMappingProfile : Profile
{
    public TableSessionMappingProfile()
    {
        CreateMap<OpenTableSessionCommand, TableSession>()
            .ForMember(session => session.Status, options => options.MapFrom(_ => TableSessionStatus.Open));
    }
}
