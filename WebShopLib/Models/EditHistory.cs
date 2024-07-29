using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("EditHistory")]
public partial class EditHistory
{
    [Key]
    public int Id { get; set; }

    public int? EmployeeId { get; set; }

    public int? Role { get; set; }

    public int? Action { get; set; }

    [Unicode(false)]
    public string? ActionInfo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TimeOfChange { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("EditHistories")]
    public virtual Employee? Employee { get; set; }
}
