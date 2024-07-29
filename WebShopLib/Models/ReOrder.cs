using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("ReOrder")]
public partial class ReOrder
{
    [Key]
    public int Id { get; set; }

    public int CreatorId { get; set; }

    [Column(TypeName = "date")]
    public DateTime OrderDate { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string ReOrderNumber { get; set; } = null!;

    public bool Sent { get; set; }

    [Column(TypeName = "date")]
    public DateTime LastStatusChange { get; set; }

    public int LastEditorId { get; set; }

    public int? SenderId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? SentDate { get; set; }

    [ForeignKey("CreatorId")]
    [InverseProperty("ReOrderCreators")]
    public virtual Employee Creator { get; set; } = null!;

    [InverseProperty("ReOrder")]
    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    [ForeignKey("LastEditorId")]
    [InverseProperty("ReOrderLastEditors")]
    public virtual Employee LastEditor { get; set; } = null!;

    [ForeignKey("SenderId")]
    [InverseProperty("ReOrderSenders")]
    public virtual Employee? Sender { get; set; }
}
