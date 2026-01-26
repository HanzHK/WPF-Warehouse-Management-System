using MediatR;
using WMS.Application.BusinessPartners.DTOs;
using WMS.Application.Addresses.DTOs;
using WMS.Domain.Repositories;

namespace WMS.Application.BusinessPartners.Queries.GetBusinessPartnerDetail;

/// <summary>
/// Handles retrieval of detailed information about a specific business partner.
/// </summary>
public sealed class GetBusinessPartnerDetailQueryHandler
    : IRequestHandler<GetBusinessPartnerDetailQuery, BusinessPartnerDetailDto>
{
    private readonly IBusinessPartnerRepository _repository;

    public GetBusinessPartnerDetailQueryHandler(IBusinessPartnerRepository repository)
    {
        _repository = repository;
    }

    public async Task<BusinessPartnerDetailDto> Handle(
        GetBusinessPartnerDetailQuery request,
        CancellationToken cancellationToken)
    {
        var partner = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (partner is null)
            throw new KeyNotFoundException($"Business partner with ID {request.Id} was not found.");

        var addressDto = new AddressDto(
            partner.Address.Id,
            partner.Address.Street,
            partner.Address.City,
            partner.Address.PostalCode,
            partner.Address.Country
        );

        return new BusinessPartnerDetailDto(
            partner.Id,
            partner.Name,
            partner.CompanyId,
            partner.VatId,
            partner.CompanyType,
            partner.Description,
            addressDto
        );
    }
}
