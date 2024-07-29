using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("Developer")]
public partial class Developer
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    public bool? IsActive { get; set; }

    [InverseProperty("Developer")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
