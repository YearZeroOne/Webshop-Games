using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

public partial class Country
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Name { get; set; }

    [StringLength(3)]
    [Unicode(false)]
    public string? Code { get; set; }

    [InverseProperty("Country")]
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
}
