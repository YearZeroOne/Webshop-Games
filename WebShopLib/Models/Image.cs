using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

public partial class Image
{
    [Key]
    public int Id { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "image")]
    public byte[] Picture { get; set; } = null!;

    public int ProductId { get; set; }

    public bool IsPrimaryPic { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Images")]
    public virtual Product Product { get; set; } = null!;
}
