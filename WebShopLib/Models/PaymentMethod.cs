using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("PaymentMethod")]
public partial class PaymentMethod
{
    [Key]
    public int Id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    public int? CreditCardId { get; set; }

    public int? DebitId { get; set; }

    [ForeignKey("CreditCardId")]
    [InverseProperty("PaymentMethods")]
    public virtual CreditCard? CreditCard { get; set; }

    [ForeignKey("DebitId")]
    [InverseProperty("PaymentMethods")]
    public virtual Debit? Debit { get; set; }

    [InverseProperty("PaymentMethod")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
