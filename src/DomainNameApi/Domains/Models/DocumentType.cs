namespace DomainNameApi.Domains;

/// <summary>Kind of registry document (mostly needed for .tr domains).</summary>
public enum DocumentType
{
    /// <summary>Identity card.</summary>
    IdentityCard = 1,

    /// <summary>Passport.</summary>
    Passport = 2,

    /// <summary>Tax certificate.</summary>
    TaxCertificate = 3,

    /// <summary>Certificate of activity.</summary>
    ActivityCertificate = 4,

    /// <summary>Signature circular.</summary>
    SignatureCircular = 5,

    /// <summary>Letter of authorisation.</summary>
    AuthorizationLetter = 6,

    /// <summary>Petition.</summary>
    Petition = 7,

    /// <summary>Trade registry gazette.</summary>
    TradeRegistryGazette = 8,

    /// <summary>Trademark certificate.</summary>
    TrademarkCertificate = 9,

    /// <summary>Education certificate (.edu.tr).</summary>
    EducationCertificate = 10,

    /// <summary>Organisation statute (.org.tr).</summary>
    OrganizationStatute = 11,

    /// <summary>Government approval (.gov.tr, .bank, .mil.tr).</summary>
    GovernmentApproval = 12,

    /// <summary>Proof of domain ownership.</summary>
    DomainOwnershipProof = 13,

    /// <summary>Power of attorney.</summary>
    PowerOfAttorney = 14,

    /// <summary>Bar association certificate (.av.tr).</summary>
    BarAssociationCertificate = 15,

    /// <summary>Medical diploma or licence (.dr.tr).</summary>
    MedicalDiplomaOrLicense = 16,

    /// <summary>Official letter of a municipality (.bel.tr).</summary>
    MunicipalOfficialLetter = 17,

    /// <summary>Licence of an education institution (.k12.tr, .edu.tr).</summary>
    EducationInstitutionLicense = 18,

    /// <summary>Chamber of commerce certificate.</summary>
    ChamberOfCommerceCertificate = 19,

    /// <summary>Trademark transfer agreement.</summary>
    TrademarkTransferAgreement = 20,

    /// <summary>KEP authorisation (.kep.tr).</summary>
    KepAuthorization = 21,

    /// <summary>Any other document.</summary>
    Other = 99,
}
