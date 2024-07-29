using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("TempEmployee")]
public partial class TempEmployee
{
    [Key]
    public int Id { get; set; }

    [Column("GUID")]
    [StringLength(36)]
    [Unicode(false)]
    public string Guid { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime TimeCreated { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [StringLength(100)]
    public string BusinessEmail { get; set; } = null!;

    [StringLength(100)]
    public string PrivateEmail { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string PrivatePhone { get; set; } = null!;

    public int Role { get; set; }

    public int CreatorId { get; set; }

    [ForeignKey("CreatorId")]
    [InverseProperty("TempEmployees")]
    public virtual Employee Creator { get; set; } = null!;
}
