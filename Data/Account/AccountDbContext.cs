using Microsoft.EntityFrameworkCore;
using PadEbankETLService.Data.DataWarehouse;

namespace PadEbankETLService.Data.Account;

public partial class AccountDbContext : DbContext {
   public AccountDbContext() { }

   public AccountDbContext(DbContextOptions<AccountDbContext> options)
      : base(options) { }

   public virtual DbSet<Card> Cards { get; set; }

   public virtual DbSet<User> Users { get; set; }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      Func<string, string> env = Environment.GetEnvironmentVariable!;

      string host = env("ACCOUNT_DB_HOST");
      string user = env("ACCOUNT_DB_USER");
      string port = env("ACCOUNT_DB_PORT");
      string db = env("ACCOUNT_DB_NAME");
      string password = env("ACCOUNT_DB_PASSWORD");
      
      optionsBuilder.UseNpgsql(
         $"Host={host};Port={port};Username={user};Password={password};Database={db};Include Error Detail=true;",
         options => { options.MapEnum<Currency>("currency", "public"); }
      );
      optionsBuilder.EnableDetailedErrors();
      optionsBuilder.EnableSensitiveDataLogging();
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.HasPostgresEnum<Currency>("public", "currency");
   }
}
