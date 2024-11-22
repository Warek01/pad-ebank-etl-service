using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PadEbankETLService.Data.Account;

[Table("users")]
public partial class User {
   [Key]
   [Column("email")]
   [StringLength(255)]
   public string Email { get; set; } = null!;

   [Column("full_name")]
   [StringLength(255)]
   public string FullName { get; set; } = null!;

   [Column("password")]
   [StringLength(255)]
   public string Password { get; set; } = null!;

   [Column("created_at", TypeName = "timestamp without time zone")]
   public DateTime CreatedAt { get; set; }

   [InverseProperty(nameof(Card.UserEmailNavigation))]
   public virtual Card? Card { get; set; }
}
