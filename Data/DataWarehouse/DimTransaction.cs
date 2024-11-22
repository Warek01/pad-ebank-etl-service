using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PadEbankETLService.Data.Transaction;
using PadEbankETLService.Helpers;

namespace PadEbankETLService.Data.DataWarehouse;

[Table("dim_transactions")]
public class DimTransaction {
   [Key]
   [Column("id")]
   public Guid Id { get; set; }

   [Column("src_card_code")]
   [StringLength(255)]
   public string? SrcCardCode { get; set; }

   [Column("dst_card_code")]
   [StringLength(255)]
   public string DstCardCode { get; set; } = null!;

   [Column("type")]
   public TransactionType Type { get; set; }
   
   [Column("currency")]
   public Currency Currency { get; set; }
   
   [Column("amount")]
   public double Amount { get; set; }

   [Column("created_at", TypeName = DbTypes.Timestamp)]
   public DateTime CreatedAt { get; set; }
   
   [InverseProperty(nameof(FactCardTransaction.DimTransaction))]
   public virtual FactCardTransaction? FactCardTransaction { get; set; }
}
