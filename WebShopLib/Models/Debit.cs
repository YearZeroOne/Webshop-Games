using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("Debit")]
public partial class Debit
{
    [Key]
    public int Id { get; set; }

    [Column("BIC")]
    [StringLength(30)]
    [Unicode(false)]
    public string Bic { get; set; } = null!;

    [Column("IBAN")]
    [StringLength(80)]
    [Unicode(false)]
    public string Iban { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [InverseProperty("Debit")]
    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
}
