namespace WMS.Domain.Common;

/// <summary>
/// Base class for all domain entities.
/// Provides a strongly-typed primary key.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Primary key of the entity.
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
