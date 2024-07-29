using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("OrderChange")]
public partial class OrderChange
{
    [Key]
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int EmployeeId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime TimeOfChange { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("OrderChanges")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("OrderChanges")]
    public virtual Order Order { get; set; } = null!;
}
