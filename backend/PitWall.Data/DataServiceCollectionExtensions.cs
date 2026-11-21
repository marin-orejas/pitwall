using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PitWall.Data;

public static class DataServiceCollectionExtensions
{
    private const string ConnectionStringName = "PitWall";

    public static IServiceCollection AddPitWallData(this IServiceCollection services) =>
        services.AddDbContext<PitWallDbContext>((provider, options) =>
            options.UseNpgsql(GetConnectionString(provider.GetRequiredService<IConfiguration>())));

    private static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"The connection string 'ConnectionStrings:{ConnectionStringName}' is not set. In development, set it with dotnet user-secrets.");
        }

        return connectionString;
    }
}
