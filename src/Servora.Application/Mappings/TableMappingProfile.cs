using AutoMapper;
using Servora.Application.Tables.CreateTable;
using Servora.Domain.Models;

namespace Servora.Application.Mappings;

public sealed class TableMappingProfile : Profile
{
    public TableMappingProfile()
    {
        CreateMap<CreateTableCommand, Table>()
            .ForMember(table => table.Name, options => options.MapFrom(command => command.Name.Trim()))
            .ForMember(table => table.TenantId, options => options.Ignore());
    }
}
