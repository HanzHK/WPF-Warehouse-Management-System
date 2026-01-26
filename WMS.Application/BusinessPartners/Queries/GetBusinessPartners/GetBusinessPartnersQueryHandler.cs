using MediatR;
using WMS.Application.BusinessPartners.DTOs;
using WMS.Domain.Repositories;

namespace WMS.Application.BusinessPartners.Queries.GetBusinessPartners;

/// <summary>
/// Handles retrieval of all business partners for list views.
/// </summary>
public sealed class GetBusinessPartnersQueryHandler
    : IRequestHandler<GetBusinessPartnersQuery, IReadOnlyList<BusinessPartnerListItemDto>>
{
    private readonly IBusinessPartnerRepository _repository;

    public GetBusinessPartnersQueryHandler(IBusinessPartnerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<BusinessPartnerListItemDto>> Handle(
        GetBusinessPartnersQuery request,
        CancellationToken cancellationToken)
    {
        var partners = await _repository.GetAllAsync(cancellationToken);

        return partners
            .Select(p => new BusinessPartnerListItemDto(
                p.Id,
                p.Name,
                p.CompanyId,
                p.CompanyType
            ))
            .ToList();
    }
}
