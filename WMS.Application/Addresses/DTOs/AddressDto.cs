using WMS.Domain.Enums;

namespace WMS.Application.Addresses.DTOs;

/// <summary>
/// Represents an address used in read operations.
/// This DTO is returned by application queries and consumed by the UI.
/// </summary>
public sealed record AddressDto(
    Guid Id,
    string Street,
    string City,
    string PostalCode,
    string Country
);
