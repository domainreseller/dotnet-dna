using DomainNameApi.Hosting;
using DomainNameApi.Products;

namespace DomainNameApi.Tests.Features;

public class HostingClientTests
{
    [Fact]
    public async Task PackagesCreateAndLookups()
    {
        using var t = new TestClient();
        t.Handler
            .Respond(200, """[{"name":"Linux Starter","requiredMinLeasing":1,"permittedMaxLeasing":36,"servicePlans":[{"servicePlan":"starter-linux","panelType":"Plesk"}]}]""")
            .Respond(202, """{"serviceId":"svc-1","status":"Provisioning","token":"tok-1"}""")
            .Respond(200, """{"hostingId":"7d9f1a52-3c1e-4c9e-9a51-6b8c2f3e4d10","serviceId":"svc-1","domainName":"example.com","expiredOn":"2027-11-19T00:00:00"}""")
            .Respond(200, """{"serviceId":"svc-1","status":"Active","panelAddress":"https://panel.example.net"}""")
            .Respond(200, """[{"name":"starter-linux","diskQuotaMb":10240,"maxDomains":null,"hasShellAccess":false}]""");

        var packages = await t.Client.Hosting.ListPackagesAsync();
        Assert.Equal("starter-linux", packages[0].ServicePlans[0].ServicePlan);

        var created = await t.Client.Hosting.CreateAsync("example.com", "starter-linux");
        Assert.Equal("hostings/create", t.LastRequest.ApiPath);
        Assert.Equal("starter-linux", t.LastRequest.Json.Str("servicePlan"));
        Assert.Equal("tok-1", created.Token);

        var hosting = await t.Client.Hosting.GetByServiceIdAsync("svc-1");
        Assert.Equal("svc-1", t.LastRequest.Query("serviceId"));
        Assert.Null(t.LastRequest.Query("domainName"));
        Assert.Equal(new DateTime(2027, 11, 19), hosting.ExpiresOn);

        var status = await t.Client.Hosting.GetStatusAsync("example.com");
        Assert.Equal("hostings/check-status", t.LastRequest.ApiPath);
        Assert.Equal("example.com", t.LastRequest.Query("DomainName"));
        Assert.Equal("https://panel.example.net", status.PanelAddress);

        var plans = await t.Client.Hosting.ListPlansAsync();
        Assert.Null(t.LastRequest.Query("hostingId"));
        Assert.Equal(10240, plans[0].DiskQuotaMb);
        Assert.Null(plans[0].MaxDomains);
    }

    [Fact]
    public async Task GetNeedsADomainOrServiceId()
    {
        using var t = new TestClient();

        await Assert.ThrowsAsync<ArgumentException>(() => t.Client.SendAsync(new GetHostingRequest()));
    }
}

public class OrdersClientTests
{
    [Fact]
    public async Task GetReadsTheOrderLines()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """
            {"orderCode":"ORD 1","status":1,"amount":"23.98","amountCurrency":"USD",
             "products":[{"productCode":"com","productType":1,"orderType":2,"status":1,"price":11.99,"period":1,"description":"example.com"}]}
            """);

        var order = await t.Client.Orders.GetAsync("ORD 1");

        Assert.Equal("/api/v1/order/ORD%201", t.LastRequest.Uri.AbsolutePath);
        Assert.Equal(Orders.OrderStatus.Active, order.Status);
        Assert.Equal(23.98m, order.Amount); // sent as a string
        var line = Assert.Single(order.Lines);
        Assert.Equal(ProductType.Domain, line.ProductType);
        Assert.Equal(OrderType.Renew, line.OrderType);
    }
}

public class ProductsClientTests
{
    private const string Tlds = """
        {"totalCount":2,"items":[
          {"name":"co","minRegistrationPeriod":1,"isDnssecAvailable":false,"prices":[{"priceGroup":"default","register":[{"period":1,"price":25,"currency":"USD"}]}]},
          {"name":"com","minRegistrationPeriod":1,"maxRegistrationPeriod":10,"isSupportTransfer":true,"isDnssecAvailable":true,
           "prices":[{"priceGroup":"default","register":[{"period":1,"price":11.99,"currency":"USD"},{"period":2,"price":23.98,"currency":"USD"}],"renew":[{"period":1,"price":12.99,"currency":"USD"}]}],
           "attributes":[{"key":"lang","isRequired":false,"options":[{"value":"en"}]}]}]}
        """;

    [Fact]
    public async Task GetPriceSendsTheOrderTypeAsANumber()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"productName":"com","productType":1,"sellingPrice":12.99,"sellingCurrency":"USD","subproducts":[]}""");

        var price = await t.Client.Products.GetPriceAsync("com", OrderType.Renew, years: 2);

        Assert.Equal("products/info", t.LastRequest.ApiPath);
        Assert.Equal("com", t.LastRequest.Query("ProductName"));
        Assert.Equal("2", t.LastRequest.Query("OrderType"));
        Assert.Equal("2", t.LastRequest.Query("Period"));
        Assert.Equal(12.99m, price.SellingPrice);
    }

    [Fact]
    public async Task TldsAndTheirPrices()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Tlds);

        var page = await t.Client.Products.ListTldsAsync(new ListTldsRequest { FilterText = "co", Currency = "USD" });

        Assert.Equal("co", t.LastRequest.Query("FilterText"));
        Assert.Equal("USD", t.LastRequest.Query("Currency"));
        var com = page.Items[1];
        Assert.True(com.SupportsTransfer);
        Assert.True(com.SupportsDnssec);
        Assert.Equal(23.98m, com.GetRegistrationPrice(2)!.Price);
        Assert.Equal(12.99m, com.GetRenewalPrice()!.Price);
        Assert.Null(com.GetRegistrationPrice(5));
        Assert.Equal("en", com.Attributes[0].Options[0].Value);
    }

    [Fact]
    public async Task GetTldFindsTheExactName()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Tlds);

        var tld = await t.Client.Products.GetTldAsync(".COM");

        Assert.Equal("com", tld!.Name);
        Assert.Equal("COM", t.LastRequest.Query("FilterText"));
    }

    [Fact]
    public async Task SslProductFilters()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"totalCount":1,"items":[{"name":"PositiveSSL","productCode":"PKG-DV","isWildcard":true,"prices":[{"priceGroup":"default","create":[{"period":1,"price":9.5,"currency":"USD"}]}]}]}""");

        var page = await t.Client.Products.ListSslProductsAsync(new ListSslProductsRequest { IsWildcard = true, VendorName = "Sectigo" });

        Assert.Equal("true", t.LastRequest.Query("IsWildcard"));
        Assert.Equal("Sectigo", t.LastRequest.Query("VendorName"));
        Assert.Null(t.LastRequest.Query("IsMultiDomain"));
        Assert.Equal(9.5m, page.Items[0].Prices[0].Purchase[0].Price);
    }

    [Fact]
    public async Task ListProductsByType()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"totalCount":0,"items":null}""");

        var page = await t.Client.Products.ListAsync(new ListProductsRequest { ProductType = ProductType.Ssl });

        Assert.Equal("2", t.LastRequest.Query("ProductType"));
        Assert.Empty(page.Items); // null in the answer
    }
}
