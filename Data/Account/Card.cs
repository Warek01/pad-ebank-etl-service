using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PadEbankETLService.Data.DataWarehouse;

namespace PadEbankETLService.Data.Account;

[Table("cards")]
[Index(nameof(UserEmail), IsUnique = true)]
public partial class Card {
   [Key]
   [Column("code")]
   [StringLength(255)]
   public string Code { get; set; } = null!;

   [Column("cvv")]
   [StringLength(3)]
   public string Cvv { get; set; } = null!;

   [Column("currency_amount")]
   public double CurrencyAmount { get; set; }

   [Column("is_blocked")]
   public bool IsBlocked { get; set; }

   [Column("created_at", TypeName = "timestamp without time zone")]
   public DateTime CreatedAt { get; set; }

   [Column("user_email")]
   [StringLength(255)]
   public string? UserEmail { get; set; }
   
   [Column("currency")]
   public Currency Currency { get; set; }

   [ForeignKey(nameof(UserEmail))]
   [InverseProperty(nameof(Card))]
   public virtual User? UserEmailNavigation { get; set; }
}
