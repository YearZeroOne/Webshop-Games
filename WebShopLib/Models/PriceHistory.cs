using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("PriceHistory")]
public partial class PriceHistory
{
    [Key]
    public int Id { get; set; }

    public int? ProductId { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime SetOn { get; set; }

    public int? SetBy { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("PriceHistories")]
    public virtual Product? Product { get; set; }

    [ForeignKey("SetBy")]
    [InverseProperty("PriceHistories")]
    public virtual Employee? SetByNavigation { get; set; }
}
