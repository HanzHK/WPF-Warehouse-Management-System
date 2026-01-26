using MediatR;
using WMS.Application.BusinessPartners.DTOs;

namespace WMS.Application.BusinessPartners.Queries.GetBusinessPartnerDetail;

/// <summary>
/// Query requesting detailed information about a specific business partner.
/// </summary>
public sealed record GetBusinessPartnerDetailQuery(Guid Id)
    : IRequest<BusinessPartnerDetailDto>;
