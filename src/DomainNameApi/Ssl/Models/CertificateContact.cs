using System.Text.Json.Serialization;

namespace DomainNameApi.Ssl;

/// <summary>
/// The contact of a certificate (sent when configuring, returned with the details). First and last name,
/// company, e-mail, address, phone, country, city and postal code are required.
/// </summary>
public sealed class CertificateContact
{
    /// <summary>Job title.</summary>
    public string? Title { get; set; }

    /// <summary>First name.</summary>
    public string? FirstName { get; set; }

    /// <summary>Last name.</summary>
    public string? LastName { get; set; }

    /// <summary>Company name.</summary>
    public string? CompanyName { get; set; }

    /// <summary>E-mail address.</summary>
    [JsonPropertyName("eMail")]
    public string? Email { get; set; }

    /// <summary>Street address.</summary>
    public string? Address { get; set; }

    /// <summary>Country calling code, digits only.</summary>
    public string? PhoneCountryCode { get; set; }

    /// <summary>Phone number, digits only.</summary>
    public string? Phone { get; set; }

    /// <summary>Fax country calling code.</summary>
    public string? FaxCountryCode { get; set; }

    /// <summary>Fax number.</summary>
    public string? Fax { get; set; }

    /// <summary>Two-letter ISO country code.</summary>
    public string? Country { get; set; }

    /// <summary>City.</summary>
    public string? City { get; set; }

    /// <summary>State or province.</summary>
    public string? State { get; set; }

    /// <summary>Postal code.</summary>
    public string? PostalCode { get; set; }

    /// <summary>The contact's role, as returned with the certificate details.</summary>
    public string? Type { get; set; }
}
