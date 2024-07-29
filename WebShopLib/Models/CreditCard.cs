using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("CreditCard")]
public partial class CreditCard
{
    [Key]
    public int Id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string FirstName { get; set; } = null!;

    [StringLength(80)]
    [Unicode(false)]
    public string LastName { get; set; } = null!;

    [StringLength(80)]
    [Unicode(false)]
    public string Number { get; set; } = null!;

    [Column("CVC")]
    public int Cvc { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string Expiration { get; set; } = null!;

    [InverseProperty("CreditCard")]
    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
}
