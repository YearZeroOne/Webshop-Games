using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("Delivery")]
public partial class Delivery
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int ReOrderId { get; set; }

    public int Quantity { get; set; }

    public bool Completed { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string DeliveryNumber { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? DeliveryDate { get; set; }

    public int? RecieverId { get; set; }

    public int CreatorId { get; set; }

    [ForeignKey("CreatorId")]
    [InverseProperty("DeliveryCreators")]
    public virtual Employee Creator { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("Deliveries")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("ReOrderId")]
    [InverseProperty("Deliveries")]
    public virtual ReOrder ReOrder { get; set; } = null!;

    [ForeignKey("RecieverId")]
    [InverseProperty("DeliveryRecievers")]
    public virtual Employee? Reciever { get; set; }
}
