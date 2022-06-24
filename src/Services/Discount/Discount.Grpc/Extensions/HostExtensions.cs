using Npgsql;

namespace Discount.Grpc.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvalability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgres database. ");

                    using (var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
                    {
                        connection.Open();

                        using (var command = new NpgsqlCommand { Connection = connection })
                        {
                            command.CommandText = "DROP TABLE IF EXISTS Coupon";
                            command.ExecuteNonQuery();

                            command.CommandText = @"CREATE TABLE Coupon(
                                                                        ID SERIAL PRIMARY KEY NOT NULL,
                                                                        ProductName VARCHAR(24) NOT NULL,
                                                                        Description TEXT,
                                                                        Amount INT
                                                                        )";

                            command.ExecuteNonQuery();

                            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('Iphone X', 'Desconto Iphone', 150)";
                            command.ExecuteNonQuery();

                            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('Samsung S22', 'Desconto Samsung', 100)";
                            command.ExecuteNonQuery();

                            logger.LogInformation("Migrated postgres database. ");
                        }
                    }
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error ocurred while migrating the postresql database");

                    if (retryForAvalability < 50)
                    {
                        retryForAvalability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvalability);
                    }
                }
            }

            return host;
        }
    }
}
