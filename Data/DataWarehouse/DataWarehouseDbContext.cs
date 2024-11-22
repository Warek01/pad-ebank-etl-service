using Microsoft.EntityFrameworkCore;
using PadEbankETLService.Data.Transaction;

namespace PadEbankETLService.Data.DataWarehouse;

public partial class DataWarehouseDbContext : DbContext {
   public DataWarehouseDbContext() { }

   public virtual DbSet<FactCardTransaction> FactCardTransactions { get; set; }
   public virtual DbSet<DimCard> DimCards { get; set; }
   public virtual DbSet<DimUser> DimUsers { get; set; }
   public virtual DbSet<DimTransaction> DimTransactions { get; set; }

   public DataWarehouseDbContext(DbContextOptions<DataWarehouseDbContext> options)
      : base(options) { }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      Func<string, string> env = Environment.GetEnvironmentVariable!;
      string host = env("DATA_WAREHOUSE_DB_HOST");
      string user = env("DATA_WAREHOUSE_DB_USER");
      string port = env("DATA_WAREHOUSE_DB_PORT");
      string db = env("DATA_WAREHOUSE_DB_NAME");
      string password = env("DATA_WAREHOUSE_DB_PASSWORD");
      
      optionsBuilder.UseNpgsql(
         $"Host={host};Port={port};Username={user};Password={password};Database={db};Include Error Detail=true;",
         options => {
            options.MapEnum<Currency>("currency", "public");
            options.MapEnum<TransactionType>("transaction_type", "public");
         }
      );
      optionsBuilder.EnableDetailedErrors();
      optionsBuilder.EnableSensitiveDataLogging();
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.HasPostgresEnum<Currency>("public", "currency");
      modelBuilder.HasPostgresEnum<TransactionType>("public", "transaction_type");
   }
}
