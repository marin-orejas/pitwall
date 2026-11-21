using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PitWall.Data;

namespace PitWall.Tests.Data;

public class PitWallDbContextTests
{
    [Fact]
    public void ResolvesWithNpgsqlProvider()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var scope = WithConnectionString(factory, "Host=localhost;Database=pitwall_test").Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<PitWallDbContext>();

        Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", context.Database.ProviderName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ThrowsWithKeyNameWhenConnectionStringIsMissing(string? connectionString)
    {
        using var factory = new WebApplicationFactory<Program>();
        using var scope = WithConnectionString(factory, connectionString).Services.CreateScope();

        var exception = Assert.Throws<InvalidOperationException>(
            () => scope.ServiceProvider.GetRequiredService<PitWallDbContext>().Database.ProviderName);

        Assert.Contains("ConnectionStrings:PitWall", exception.Message, StringComparison.Ordinal);
    }

    private static WebApplicationFactory<Program> WithConnectionString(WebApplicationFactory<Program> factory, string? connectionString) =>
        factory.WithWebHostBuilder(host => host.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:PitWall"] = connectionString })));
}
