using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

[Table("Address")]
public partial class Address
{
    [Key]
    public int Id { get; set; }

    public int CustomerId { get; set; }

    [StringLength(100)]
    public string Country { get; set; } = null!;

    [StringLength(100)]
    public string City { get; set; } = null!;

    [Column("ZIPCode")]
    public int Zipcode { get; set; }

    [StringLength(400)]
    public string AddressLine { get; set; } = null!;

    public bool IsBillingAddress { get; set; }

    public bool? IsDeliveryAddress { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("Addresses")]
    public virtual Customer Customer { get; set; } = null!;
}
