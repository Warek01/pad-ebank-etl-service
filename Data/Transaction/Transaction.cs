using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PadEbankETLService.Data.DataWarehouse;

namespace PadEbankETLService.Data.Transaction;

[Table("transactions")]
public partial class Transaction {
   [Key]
   [Column("id")]
   public Guid Id { get; set; }

   [Column("src_card_code")]
   [StringLength(255)]
   public string? SrcCardCode { get; set; }

   [Column("dst_card_code")]
   [StringLength(255)]
   public string? DstCardCode { get; set; }

   [Column("amount")]
   public int Amount { get; set; }
   
   [Column("currency")]
   public Currency Currency { get; set; }
   
   [Column("type")]
   public TransactionType Type { get; set; }

   [Column("created_at", TypeName = "timestamp without time zone")]
   public DateTime CreatedAt { get; set; }
}
