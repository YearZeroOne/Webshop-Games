using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("Order")]
public partial class Order
{
    [Key]
    public int Id { get; set; }

    public int CustomerId { get; set; }

    [Column(TypeName = "date")]
    public DateTime OrderDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? DeliveryDate { get; set; }

    [StringLength(12)]
    [Unicode(false)]
    public string OrderNumber { get; set; } = null!;

    public bool PaymentPending { get; set; }

    public int PaymentMethodId { get; set; }

    public bool? PickUp { get; set; }

    public bool Cancelled { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Orders")]
    public virtual Customer Customer { get; set; } = null!;

    [InverseProperty("Order")]
    public virtual ICollection<OrderChange> OrderChanges { get; set; } = new List<OrderChange>();

    [InverseProperty("Order")]
    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    [ForeignKey("PaymentMethodId")]
    [InverseProperty("Orders")]
    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
