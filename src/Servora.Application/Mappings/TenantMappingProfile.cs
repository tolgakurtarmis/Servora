using AutoMapper;
using Servora.Application.Tenants.CreateTenant;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Mappings;

public sealed class TenantMappingProfile : Profile
{
    public TenantMappingProfile()
    {
        CreateMap<CreateTenantCommand, Tenant>()
            .ForMember(tenant => tenant.Name, options => options.MapFrom(command => command.Name.Trim()));

        CreateMap<CreateTenantCommand, Subscription>()
            .ForMember(
                subscription => subscription.Status,
                options => options.MapFrom(command => command.Status ?? SubscriptionStatus.Trial))
            .ForMember(subscription => subscription.TenantId, options => options.Ignore());
    }
}
