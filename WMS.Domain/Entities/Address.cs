using WMS.Domain.Common;

namespace WMS.Domain.Entities;

/// <summary>
/// Represents a physical address.
/// </summary>
public class Address : BaseEntity
{
    /// <summary>
    /// Street name.
    /// </summary>
    public string Street { get; private set; } = null!;

    /// <summary>
    /// Building number.
    /// </summary>
    public string BuildingNumber { get; private set; } = null!;

    /// <summary>
    /// Postal code.
    /// </summary>
    public string PostalCode { get; private set; } = null!;

    /// <summary>
    /// City or municipality.
    /// </summary>
    public string City { get; private set; } = null!;

    /// <summary>
    /// Reference to the country.
    /// </summary>
    public Guid CountryId { get; private set; }

    /// <summary>
    /// Navigation property for the country.
    /// </summary>
    public Country Country { get; private set; } = null!;

    public Address(
        string street,
        string buildingNumber,
        string postalCode,
        string city,
        Country country)
    {
        Street = street;
        BuildingNumber = buildingNumber;
        PostalCode = postalCode;
        City = city;
        Country = country;
        CountryId = country.Id;
    }

    private Address() { }
}
