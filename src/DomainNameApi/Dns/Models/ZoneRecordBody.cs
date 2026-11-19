namespace DomainNameApi.Dns;

/// <summary>The <c>{"zoneStruct": {...}}</c> body of the zone write endpoints.</summary>
internal sealed class ZoneRecordBody
{
    public ZoneRecordBody(string? name, RecordType type, int ttl, IReadOnlyList<string> contents)
    {
        ZoneStruct = new ZoneStructBody { Name = name, Type = type, Ttl = ttl, Contents = contents };
    }

    public ZoneStructBody ZoneStruct { get; }

    internal sealed class ZoneStructBody
    {
        public string? Name { get; set; }

        public int Ttl { get; set; }

        public RecordType Type { get; set; }

        public IReadOnlyList<string> Contents { get; set; } = [];
    }
}
