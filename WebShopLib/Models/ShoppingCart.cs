using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("ShoppingCart")]
[Index("CustomerId", Name = "CustomerUQ", IsUnique = true)]
public partial class ShoppingCart
{
    [Key]
    public int Id { get; set; }

    public int CustomerId { get; set; }

    [InverseProperty("ShoppingCart")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [ForeignKey("CustomerId")]
    [InverseProperty("ShoppingCart")]
    public virtual Customer Customer { get; set; } = null!;
}
