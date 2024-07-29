using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

public partial class ReOrderProduct
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int ReOrderId { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ReOrderProducts")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("ReOrderId")]
    [InverseProperty("ReOrderProducts")]
    public virtual ReOrder ReOrder { get; set; } = null!;
}
