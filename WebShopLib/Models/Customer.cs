using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("Customer")]
public partial class Customer
{
    [Key]
    public int Id { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string CustomerNumber { get; set; } = null!;

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    public int Gender { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Phone { get; set; } = null!;

    [StringLength(80)]
    [Unicode(false)]
    public string HashPass { get; set; } = null!;

    [StringLength(80)]
    [Unicode(false)]
    public string Salt { get; set; } = null!;

    [Column("GUID")]
    [StringLength(36)]
    [Unicode(false)]
    public string? Guid { get; set; }

    public bool IsActive { get; set; }

    public int AmountOrders { get; set; }

    [Column(TypeName = "date")]
    public DateTime? LastOrderDate { get; set; }

    public bool IsLocked { get; set; }

    public int UnsuccessfullLogin { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastLogin { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime RegistrationDate { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    [InverseProperty("Customer")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("Customer")]
    public virtual ICollection<PasswordReset> PasswordResets { get; set; } = new List<PasswordReset>();

    [InverseProperty("Customer")]
    public virtual ShoppingCart? ShoppingCart { get; set; }
}
