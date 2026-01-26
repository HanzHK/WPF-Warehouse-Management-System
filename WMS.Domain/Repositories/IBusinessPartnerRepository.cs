using WMS.Domain.Entities;

namespace WMS.Domain.Repositories;

/// <summary>
/// Repository abstraction for accessing business partners.
/// </summary>
public interface IBusinessPartnerRepository
{
    Task<IReadOnlyList<BusinessPartner>> GetAllAsync(CancellationToken cancellationToken);
    Task<BusinessPartner?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
