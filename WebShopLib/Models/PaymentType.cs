using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("PaymentType")]
public partial class PaymentType
{
    [Key]
    public int Id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [InverseProperty("PaymentType")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
