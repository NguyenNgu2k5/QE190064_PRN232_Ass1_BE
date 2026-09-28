using Npgsql;

namespace TaskTrack.API;

public static class DatabaseConnection
{
    public static string Resolve(IConfiguration configuration)
    {
        var value = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(value)) value = configuration["DATABASE_URL"];
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException("Set ConnectionStrings:DefaultConnection or DATABASE_URL before starting the API.");
        if (!value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) && !value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)) return value;
        var uri = new Uri(value);
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Username = Uri.UnescapeDataString(uri.UserInfo.Split(':')[0]),
            Password = Uri.UnescapeDataString(uri.UserInfo.Split(':').ElementAtOrDefault(1) ?? string.Empty),
            Database = uri.AbsolutePath.Trim('/'),
            SslMode = SslMode.Require
        };
        return builder.ConnectionString;
    }
}
