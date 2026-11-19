using System;
using System.Runtime.CompilerServices;

namespace DomainNameApi.Tests
{
    /// <summary>
    /// The default DSN points at DomainNameAPI's real Sentry project. Before any test
    /// runs, every client's default telemetry sender is replaced with one that drops
    /// the event, so the test suite can never post to the real project — whether a
    /// test disables reporting, keeps the default DSN or sets its own.
    /// Tests that inspect events assign their own <c>TelemetrySender</c>.
    /// </summary>
    internal static class TelemetryTestGuard
    {
        public static readonly Action<string, string, string> DropEvent = (url, body, auth) => { };

        [ModuleInitializer]
        internal static void Install() => DomainNameApiClient.DefaultTelemetrySender = DropEvent;
    }
}
