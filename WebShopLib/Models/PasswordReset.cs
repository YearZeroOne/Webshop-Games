using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("PasswordReset")]
public partial class PasswordReset
{
    [Key]
    public int Id { get; set; }

    [Column("GUID")]
    [StringLength(36)]
    [Unicode(false)]
    public string? Guid { get; set; }

    public int? CustomerId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TimeCreator { get; set; }

    public int? EmployeeId { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("PasswordResets")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("PasswordResets")]
    public virtual Employee? Employee { get; set; }
}
