namespace DomainNameApi.Domains;

/// <summary>What to do with DNSSEC when the nameservers of a signed domain change.</summary>
public enum DnssecMode
{
    /// <summary>Refuse the nameserver change while DS records exist. The safe default.</summary>
    Block = 0,

    /// <summary>Keep the DS records; you confirm the new DNS provider serves a matching DNSKEY.</summary>
    KeepDnssec = 1,

    /// <summary>Remove all DS records at the registry first, then change the nameservers.</summary>
    DisableDnssec = 2,
}
