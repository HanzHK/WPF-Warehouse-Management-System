using WMS.Domain.Common;

namespace WMS.Domain.Entities;

/// <summary>
/// Represents a country used for addressing.
/// </summary>
public class Country : BaseEntity
{
    /// <summary>
    /// ISO country code (e.g., CZ, SK, DE).
    /// </summary>
    public string Code { get; private set; } = null!;

    /// <summary>
    /// Full country name.
    /// </summary>
    public string Name { get; private set; } = null!;

    public Country(string code, string name)
    {
        Code = code;
        Name = name;
    }

    private Country() { }
}
