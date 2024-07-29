using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("Payment")]
public partial class Payment
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal Amount { get; set; }

    public int PaymentTypeId { get; set; }

    public int OrderId { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("Payments")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("PaymentTypeId")]
    [InverseProperty("Payments")]
    public virtual PaymentType PaymentType { get; set; } = null!;
}
