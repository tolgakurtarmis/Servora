using AutoMapper;
using Servora.Application.Branches.CreateBranch;
using Servora.Domain.Models;

namespace Servora.Application.Mappings;

public sealed class BranchMappingProfile : Profile
{
    public BranchMappingProfile()
    {
        CreateMap<CreateBranchCommand, Branch>()
            .ForMember(branch => branch.Name, options => options.MapFrom(command => command.Name.Trim()));
    }
}
