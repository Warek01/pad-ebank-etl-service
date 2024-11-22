using Microsoft.EntityFrameworkCore;
using PadEbankETLService.Data.Account;
using PadEbankETLService.Data.DataWarehouse;
using PadEbankETLService.Data.Transaction;

namespace PadEbankETLService.Services;

public static class ETLService {
   public static async Task Init() {
      Console.WriteLine("ETL service initializing");
      try {
         TimeSpan duration = await Perform();
         Console.WriteLine($"ETL complete (took {duration.Milliseconds / 1000.0:F2} seconds)");
      }
      catch (Exception ex) {
         Console.WriteLine("ETL initialization failed");
         Console.WriteLine(ex);
      }

      _ = Schedule();
   }

   public static async Task<TimeSpan> Perform() {
      DateTime start = DateTime.Now;
      Console.WriteLine($"Performing ETL ({start})");

      var accountDb = new AccountDbContext();
      var transactionDb = new TransactionDbContext();
      var dataWarehouseDb = new DataWarehouseDbContext();

      await dataWarehouseDb.Database.BeginTransactionAsync();

      try {
         List<Transaction> transactions = await transactionDb.Transactions.ToListAsync();
         Console.WriteLine($"Extracted transactions from {transactionDb.Database.GetDbConnection().Database}");

         foreach (Transaction transaction in transactions) {
            if (await dataWarehouseDb.FactCardTransactions.AnyAsync(c => c.Id == transaction.Id)) {
               Console.WriteLine($"Skipping {transaction.Id}");
               continue;
            }

            Card card = await accountDb.Cards
               .Include(c => c.UserEmailNavigation)
               .Where(c => c.Code == transaction.DstCardCode)
               .FirstAsync();
            User user = card.UserEmailNavigation!;

            var factCardTransaction = new FactCardTransaction {
               Id = transaction.Id,
               UserEmail = user.Email,
               Amount = transaction.Amount,
               Currency = transaction.Currency,
               CreatedAt = DateTime.Now,
               DstCardCode = card.Code,
            };

            DimCard? dimCard = await dataWarehouseDb.DimCards
               .Where(c => c.Code == card.Code)
               .FirstOrDefaultAsync();

            if (dimCard is null) {
               dimCard = new DimCard {
                  Code = card.Code,
                  Email = user.Email,
                  CreatedAt = card.CreatedAt,
               };
               await dataWarehouseDb.DimCards.AddAsync(dimCard);
            }

            dimCard.FactCardTransactions.Add(factCardTransaction);
            factCardTransaction.DimCard = dimCard;

            DimUser? dimUser = await dataWarehouseDb.DimUsers
               .Where(u => u.Email == card.UserEmail)
               .FirstOrDefaultAsync();

            if (dimUser is null) {
               dimUser = new DimUser {
                  Email = user.Email,
                  CreatedAt = user.CreatedAt,
                  FullName = user.FullName,
               };
               await dataWarehouseDb.DimUsers.AddAsync(dimUser);
            }

            factCardTransaction.DimUser = dimUser;

            var dimTransaction = new DimTransaction {
               Id = transaction.Id,
               Currency = transaction.Currency,
               Amount = transaction.Amount,
               DstCardCode = transaction.DstCardCode!,
               CreatedAt = transaction.CreatedAt,
               Type = transaction.Type,
               SrcCardCode = transaction.SrcCardCode,
               FactCardTransaction = factCardTransaction,
            };
            await dataWarehouseDb.DimTransactions.AddAsync(dimTransaction);
            factCardTransaction.DimTransaction = dimTransaction;

            await dataWarehouseDb.FactCardTransactions.AddAsync(factCardTransaction);
            await dataWarehouseDb.SaveChangesAsync();
            await dataWarehouseDb.Database.CommitTransactionAsync();
         }
      }
      catch (Exception ex) {
         await dataWarehouseDb.Database.RollbackTransactionAsync();
         Console.WriteLine("ETL error");
         Console.WriteLine(ex);
      }

      return DateTime.Now - start;
   }

   private static async Task Schedule() {
      while (true) {
         await Task.Delay(TimeSpan.FromSeconds(60));
         Console.WriteLine("Running a scheduled ETL");
         try {
            TimeSpan duration = await Perform();
            Console.WriteLine($"ETL complete (took {duration.Milliseconds / 1000.0:F2} seconds)");
         }
         catch (Exception ex) {
            Console.WriteLine("Scheduled ETL failed");
            Console.WriteLine(ex);
         }
      }
   }
}
