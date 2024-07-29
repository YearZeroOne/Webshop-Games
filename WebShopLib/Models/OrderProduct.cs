using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

public partial class OrderProduct
{
    [Key]
    public int Id { get; set; }

    public int ProductsId { get; set; }

    public int OrderId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderProducts")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("ProductsId")]
    [InverseProperty("OrderProducts")]
    public virtual Product Products { get; set; } = null!;
}
