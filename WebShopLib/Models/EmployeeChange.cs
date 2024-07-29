using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("EmployeeChange")]
public partial class EmployeeChange
{
    [Key]
    public int Id { get; set; }

    public int ExecutingUser { get; set; }

    public int ExecutingUserRole { get; set; }

    public int AffectedUser { get; set; }

    public int Action { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? ActionInformation { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime TimeOfChange { get; set; }

    [ForeignKey("AffectedUser")]
    [InverseProperty("EmployeeChangeAffectedUserNavigations")]
    public virtual Employee AffectedUserNavigation { get; set; } = null!;

    [ForeignKey("ExecutingUser")]
    [InverseProperty("EmployeeChangeExecutingUserNavigations")]
    public virtual Employee ExecutingUserNavigation { get; set; } = null!;
}
