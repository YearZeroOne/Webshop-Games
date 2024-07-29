using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("Store")]
public partial class Store
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string Address { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? BankName { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? BankAccount { get; set; }
}
