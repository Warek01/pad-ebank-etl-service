using Microsoft.EntityFrameworkCore;
using PadEbankETLService.Data.DataWarehouse;

namespace PadEbankETLService.Data.Transaction;

public partial class TransactionDbContext : DbContext {
   public TransactionDbContext() { }

   public TransactionDbContext(DbContextOptions<TransactionDbContext> options)
      : base(options) { }

   public virtual DbSet<Transaction> Transactions { get; set; }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      Func<string, string> env = Environment.GetEnvironmentVariable!;

      string host = env("TRANSACTION_DB_HOST");
      string user = env("TRANSACTION_DB_USER");
      string port = env("TRANSACTION_DB_PORT");
      string db = env("TRANSACTION_DB_NAME");
      string password = env("TRANSACTION_DB_PASSWORD");
      
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
      modelBuilder
         .HasPostgresEnum<Currency>("public", "currency")
         .HasPostgresEnum<TransactionType>("public", "transaction_type")
         .HasPostgresExtension("uuid-ossp");
      
      modelBuilder.Entity<Transaction>(entity => {
         entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
      });
   }
}
