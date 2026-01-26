using WMS.Domain.Enums;

namespace WMS.Application.BusinessPartners.DTOs;

/// <summary>
/// Represents a business partner entry used in list views.
/// Contains only the essential information required for displaying
/// a partner in a table or overview.
/// </summary>
public sealed record BusinessPartnerListItemDto(
    Guid Id,
    string Name,
    string CompanyId,
    BusinessPartnerType CompanyType
);

