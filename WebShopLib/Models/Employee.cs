using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Index("UserName", Name = "userNameUnique", IsUnique = true)]
public partial class Employee
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(100)]
    public string BusinessEmail { get; set; } = null!;

    [StringLength(100)]
    public string PrivateEmail { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string PrivatePhone { get; set; } = null!;

    [Column("GUID")]
    [StringLength(36)]
    [Unicode(false)]
    public string? Guid { get; set; }

    public int Role { get; set; }

    [StringLength(80)]
    [Unicode(false)]
    public string HashPassword { get; set; } = null!;

    [StringLength(80)]
    [Unicode(false)]
    public string Salt { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsLocked { get; set; }

    public int UnsuccessfullLogin { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [InverseProperty("Creator")]
    public virtual ICollection<Delivery> DeliveryCreators { get; set; } = new List<Delivery>();

    [InverseProperty("Reciever")]
    public virtual ICollection<Delivery> DeliveryRecievers { get; set; } = new List<Delivery>();

    [InverseProperty("AffectedUserNavigation")]
    public virtual ICollection<EmployeeChange> EmployeeChangeAffectedUserNavigations { get; set; } = new List<EmployeeChange>();

    [InverseProperty("ExecutingUserNavigation")]
    public virtual ICollection<EmployeeChange> EmployeeChangeExecutingUserNavigations { get; set; } = new List<EmployeeChange>();

    [InverseProperty("Employee")]
    public virtual ICollection<OrderChange> OrderChanges { get; set; } = new List<OrderChange>();

    [InverseProperty("Employee")]
    public virtual ICollection<PasswordReset> PasswordResets { get; set; } = new List<PasswordReset>();

    [InverseProperty("SetByNavigation")]
    public virtual ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();

    [InverseProperty("Creator")]
    public virtual ICollection<ReOrder> ReOrderCreators { get; set; } = new List<ReOrder>();

    [InverseProperty("LastEditor")]
    public virtual ICollection<ReOrder> ReOrderLastEditors { get; set; } = new List<ReOrder>();

    [InverseProperty("Sender")]
    public virtual ICollection<ReOrder> ReOrderSenders { get; set; } = new List<ReOrder>();

    [InverseProperty("Creator")]
    public virtual ICollection<TempEmployee> TempEmployees { get; set; } = new List<TempEmployee>();
}
