using System.Text.Json.Serialization;

namespace DomainNameApi.Contacts;

/// <summary>
/// The details of a contact. Every field except <see cref="CompanyName"/>, the fax fields and
/// <see cref="DiscloseFlag"/> is required when you create or update one.
/// </summary>
/// <example>
/// <code>
/// var contact = new ContactInfo
/// {
///     FirstName = "Ada",
///     LastName = "Lovelace",
///     Email = "ada@example.com",
///     Address = "1 Example Street",
///     City = "Istanbul",
///     State = "Istanbul",
///     PostalCode = "34000",
///     Country = "TR",
///     PhoneCountryCode = "90",
///     Phone = "5551234567",
/// };
/// </code>
/// </example>
public class ContactInfo
{
    /// <summary>Creates an empty contact.</summary>
    public ContactInfo()
    {
    }

    /// <summary>Copies the fields of another contact.</summary>
    /// <param name="other">The contact to copy.</param>
    public ContactInfo(ContactInfo other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        FirstName = other.FirstName;
        LastName = other.LastName;
        CompanyName = other.CompanyName;
        Email = other.Email;
        Address = other.Address;
        PhoneCountryCode = other.PhoneCountryCode;
        Phone = other.Phone;
        FaxCountryCode = other.FaxCountryCode;
        Fax = other.Fax;
        PostalCode = other.PostalCode;
        Country = other.Country;
        City = other.City;
        State = other.State;
        DiscloseFlag = other.DiscloseFlag;
    }

    /// <summary>First name (at least 2 letters).</summary>
    public string? FirstName { get; set; }

    /// <summary>Last name (at least 2 letters).</summary>
    public string? LastName { get; set; }

    /// <summary>Company name. The API uses the first and last name when it is empty.</summary>
    public string? CompanyName { get; set; }

    /// <summary>E-mail address.</summary>
    [JsonPropertyName("eMail")]
    public string? Email { get; set; }

    /// <summary>Street address.</summary>
    public string? Address { get; set; }

    /// <summary>Country calling code, digits only without <c>+</c>, for example <c>90</c>.</summary>
    public string? PhoneCountryCode { get; set; }

    /// <summary>Phone number, digits only, for example <c>5551234567</c>.</summary>
    public string? Phone { get; set; }

    /// <summary>Fax country calling code, digits only.</summary>
    public string? FaxCountryCode { get; set; }

    /// <summary>Fax number, digits only.</summary>
    public string? Fax { get; set; }

    /// <summary>Postal code.</summary>
    public string? PostalCode { get; set; }

    /// <summary>Two-letter ISO country code, for example <c>TR</c> or <c>US</c>.</summary>
    public string? Country { get; set; }

    /// <summary>City.</summary>
    public string? City { get; set; }

    /// <summary>State or province.</summary>
    public string? State { get; set; }

    /// <summary>Whether to publish the contact in WHOIS. Null keeps the registry's default.</summary>
    public bool? DiscloseFlag { get; set; }

    /// <summary>Throws an <see cref="ArgumentException"/> naming every missing required field.</summary>
    /// <param name="name">The parameter name used in the error message.</param>
    public void EnsureComplete(string name = "contact")
    {
        var missing = new List<string>();
        void Check(string? value, string field)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                missing.Add(field);
            }
        }
        Check(FirstName, nameof(FirstName));
        Check(LastName, nameof(LastName));
        Check(Email, nameof(Email));
        Check(Address, nameof(Address));
        Check(City, nameof(City));
        Check(State, nameof(State));
        Check(PostalCode, nameof(PostalCode));
        Check(Country, nameof(Country));
        Check(PhoneCountryCode, nameof(PhoneCountryCode));
        Check(Phone, nameof(Phone));
        if (missing.Count > 0)
        {
            throw new ArgumentException(name + " is missing required fields: " + string.Join(", ", missing) + ".", name);
        }
    }
}
