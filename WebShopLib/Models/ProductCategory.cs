using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("ProductCategory")]
public partial class ProductCategory
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "decimal(3, 0)")]
    public decimal TaxRate { get; set; }

    [InverseProperty("ProductCategory")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
