using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

public partial class CartItem
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int ShoppingCartId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("CartItems")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("ShoppingCartId")]
    [InverseProperty("CartItems")]
    public virtual ShoppingCart ShoppingCart { get; set; } = null!;
}
