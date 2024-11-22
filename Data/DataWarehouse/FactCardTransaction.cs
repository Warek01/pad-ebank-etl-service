using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PadEbankETLService.Helpers;

namespace PadEbankETLService.Data.DataWarehouse;

[Table("fact_card_transactions")]
public class FactCardTransaction {
   [Key]
   [Column("id")]
   public Guid Id { get; set; }

   [Column("currency")]
   public Currency Currency { get; set; }

   [Column("amount")]
   public double Amount { get; set; }

   [Column("created_at", TypeName = DbTypes.Timestamp)]
   public DateTime CreatedAt { get; set; }

   [Column("email")]
   [StringLength(255)]
   public string UserEmail { get; set; } = null!;

   [Column("dst_card_code")]
   [StringLength(255)]
   public string DstCardCode { get; set; } = null!;

   [InverseProperty(nameof(DimCard.FactCardTransactions))]
   [ForeignKey(nameof(UserEmail))]
   public virtual DimUser? DimUser { get; set; }

   [InverseProperty(nameof(DimCard.FactCardTransactions))]
   [ForeignKey(nameof(DstCardCode))]
   public virtual DimCard? DimCard { get; set; }
   
   [InverseProperty(nameof(DimTransaction.FactCardTransaction))]
   [ForeignKey(nameof(Id))]
   public virtual DimTransaction? DimTransaction { get; set; }
}
