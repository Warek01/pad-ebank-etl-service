using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PadEbankETLService.Helpers;

namespace PadEbankETLService.Data.DataWarehouse;

[Table("dim_cards")]
public class DimCard {
   [Key]
   [Column("card_code")]
   [StringLength(255)]
   public string Code { get; set; } = null!;

   [Column("email")]
   [StringLength(255)]
   public string Email { get; set; } = null!;
   
   [Column("currency")]
   public Currency Currency { get; set; }
   
   [Column("created_at", TypeName = DbTypes.Timestamp)]
   public DateTime CreatedAt { get; set; }

   [InverseProperty(nameof(FactCardTransaction.DimCard))]
   public virtual List<FactCardTransaction> FactCardTransactions { get; set; } = [];
}
