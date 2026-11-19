namespace DomainNameApi;

/// <summary>
/// The API deployment a client talks to. Each environment has its own data, balance and API key.
/// </summary>
public enum ApiEnvironment
{
    /// <summary>The live environment (<c>https://api.domainresellerapi.com</c>): real domains and real charges.</summary>
    Production,

    /// <summary>
    /// The test environment (<c>https://ote.domainresellerapi.com</c>), also called OTE. Use your OTE API key
    /// with it; nothing you do there touches live domains or your live balance.
    /// </summary>
    Ote,
}

/// <summary>Helpers for <see cref="ApiEnvironment"/>.</summary>
public static class ApiEnvironments
{
    /// <summary>Base address of the production environment.</summary>
    public static readonly Uri ProductionBaseAddress = new("https://api.domainresellerapi.com/");

    /// <summary>Base address of the test (OTE) environment.</summary>
    public static readonly Uri OteBaseAddress = new("https://ote.domainresellerapi.com/");

    /// <summary>Returns the base address of an environment.</summary>
    public static Uri BaseAddress(this ApiEnvironment environment) =>
        environment == ApiEnvironment.Ote ? OteBaseAddress : ProductionBaseAddress;

    /// <summary>
    /// Reads an environment name. Accepts <c>production</c>, <c>prod</c>, <c>live</c> and <c>ote</c>,
    /// <c>test</c>, <c>sandbox</c>, as well as the test-mode flags other DomainNameAPI tools use
    /// (<c>true</c>, <c>1</c>, <c>on</c>, <c>yes</c> mean OTE; <c>false</c>, <c>0</c>, <c>off</c>, <c>no</c> mean production).
    /// An empty value means production.
    /// </summary>
    /// <param name="value">The text to read.</param>
    /// <param name="environment">The environment, when the text was recognised.</param>
    /// <returns>Whether the text was recognised.</returns>
    public static bool TryParse(string? value, out ApiEnvironment environment)
    {
        switch ((value ?? "").Trim().ToLowerInvariant())
        {
            case "":
            case "production":
            case "prod":
            case "live":
            case "false":
            case "0":
            case "off":
            case "no":
                environment = ApiEnvironment.Production;
                return true;
            case "ote":
            case "test":
            case "testing":
            case "sandbox":
            case "true":
            case "1":
            case "on":
            case "yes":
                environment = ApiEnvironment.Ote;
                return true;
            default:
                environment = ApiEnvironment.Production;
                return false;
        }
    }
}
