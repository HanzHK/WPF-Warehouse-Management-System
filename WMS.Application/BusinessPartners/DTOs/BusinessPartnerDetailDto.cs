using WMS.Domain.Enums;
using WMS.Application.Addresses.DTOs;

namespace WMS.Application.BusinessPartners.DTOs;

/// <summary>
/// Represents a detailed view of a business partner.
/// Used when displaying full partner information in a detail screen.
/// </summary>
public sealed record BusinessPartnerDetailDto(
    Guid Id,
    string Name,
    string CompanyId,
    string VatId,
    BusinessPartnerType CompanyType,
    string? Description,
    AddressDto Address
);
