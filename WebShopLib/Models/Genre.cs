using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("Genre")]
public partial class Genre
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Name { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Genre")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
