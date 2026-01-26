using MediatR;
using WMS.Application.BusinessPartners.DTOs;

namespace WMS.Application.BusinessPartners.Queries.GetBusinessPartners;

/// <summary>
/// Query requesting a list of all business partners.
/// </summary>
public sealed record GetBusinessPartnersQuery
    : IRequest<IReadOnlyList<BusinessPartnerListItemDto>>;
