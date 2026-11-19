namespace DomainNameApi.Tests.Infrastructure;

internal static class JsonElementExtensions
{
    public static string? Str(this JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind != JsonValueKind.Null ? value.ToString() : null;

    public static bool Has(this JsonElement element, string name) => element.TryGetProperty(name, out _);

    public static IReadOnlyList<string> Names(this JsonElement element) =>
        element.EnumerateObject().Select(p => p.Name).ToList();
}
