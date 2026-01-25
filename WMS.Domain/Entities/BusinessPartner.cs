using WMS.Domain.Common;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities;

/// <summary>
/// Represents a business partner involved in warehouse operations.
/// Can act as a supplier, recipient, customer, or any other role.
/// </summary>
public class BusinessPartner : BaseEntity
{
    /// <summary>
    /// Full legal name of the business partner.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Company identification number (IČO).
    /// </summary>
    public string CompanyId { get; private set; } = null!;

    /// <summary>
    /// VAT identification number (DIČ).
    /// </summary>
    public string VatId { get; private set; } = null!;

    /// <summary>
    /// Legal form of the business partner (e.g., s.r.o., a.s., sole trader).
    /// </summary>
    public BusinessPartnerType CompanyType { get; private set; } 

    /// <summary>
    /// Short description or internal note.
    /// </summary>
    public string? Description { get; private set; } = null!;

    /// <summary>
    /// Reference to the partner's address.
    /// </summary>
    public Guid AddressId { get; private set; } 

    /// <summary>
    /// Navigation property for the partner's address.
    /// </summary>
    public Address Address { get; private set; } = null!;

    /// <summary>
    /// Creates a new business partner instance.
    /// </summary>
    public BusinessPartner(
        string name,
        string companyId,
        string vatId,
        BusinessPartnerType companyType,
        string? description,
        Address address)
    {
        Name = name;
        CompanyId = companyId;
        VatId = vatId;
        CompanyType = companyType;
        Description = description;
        Address = address;
        AddressId = address.Id;
    }
    private BusinessPartner() { }
}
