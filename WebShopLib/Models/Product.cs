using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

public partial class Product
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string ProductNumber { get; set; } = null!;

    public int ProductCategoryId { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal NetUnitPrice { get; set; }

    public int Stock { get; set; }

    public bool IsActive { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string Description { get; set; } = null!;

    public int GenreId { get; set; }

    public int DeveloperId { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [InverseProperty("Product")]
    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    [ForeignKey("DeveloperId")]
    [InverseProperty("Products")]
    public virtual Developer Developer { get; set; } = null!;

    [ForeignKey("GenreId")]
    [InverseProperty("Products")]
    public virtual Genre Genre { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    [InverseProperty("Products")]
    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    [InverseProperty("Product")]
    public virtual ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();

    [ForeignKey("ProductCategoryId")]
    [InverseProperty("Products")]
    public virtual ProductCategory ProductCategory { get; set; } = null!;
}
