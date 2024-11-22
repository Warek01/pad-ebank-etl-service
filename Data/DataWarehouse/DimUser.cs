using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PadEbankETLService.Helpers;

namespace PadEbankETLService.Data.DataWarehouse;

[Table("dim_users")]
public class DimUser {
   [Key]
   [Column("email")]
   [StringLength(255)]
   public string Email { get; set; } = null!;

   [Column("full_name")]
   [StringLength(255)]
   public string FullName { get; set; } = null!;

   [Column("created_at", TypeName = DbTypes.Timestamp)]
   public DateTime CreatedAt { get; set; }

   [InverseProperty(nameof(FactCardTransaction.DimUser))]
   public virtual List<FactCardTransaction> FactCardTransactions { get; set; } = [];
}
