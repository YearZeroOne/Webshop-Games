using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebShopLib.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<CreditCard> CreditCards { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Debit> Debits { get; set; }

    public virtual DbSet<Delivery> Deliveries { get; set; }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeChange> EmployeeChanges { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderChange> OrderChanges { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<PasswordReset> PasswordResets { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PriceHistory> PriceHistories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ReOrder> ReOrders { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<TempEmployee> TempEmployees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-NSNNNML\\SQLEXPRESS;Initial Catalog=Webshop;Persist Security Info=True;User ID=sa;Password=Admin2019$; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Address__3214EC0727B1EDFC");

            entity.Property(e => e.FirstName).HasDefaultValueSql("('John')");
            entity.Property(e => e.LastName).HasDefaultValueSql("('Doe')");

            entity.HasOne(d => d.Customer).WithMany(p => p.Addresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Address__Custome__68487DD7");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CartItem__3214EC0778B8C4B6");

            entity.Property(e => e.Price).HasDefaultValueSql("((20))");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItems_Product");

            entity.HasOne(d => d.ShoppingCart).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItems_ShoppingCart");
        });

        modelBuilder.Entity<CreditCard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CreditCa__3214EC0729D2BB17");

            entity.Property(e => e.Expiration).IsFixedLength();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07FE0E061A");

            entity.Property(e => e.Guid).IsFixedLength();
        });

        modelBuilder.Entity<Debit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Debit__3214EC07667A1F45");
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Delivery__3214EC077C05522E");

            entity.Property(e => e.CreatorId).HasDefaultValueSql("((2))");
            entity.Property(e => e.DeliveryNumber).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Creator).WithMany(p => p.DeliveryCreators)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Delivery__Creato__3429BB53");

            entity.HasOne(d => d.Product).WithMany(p => p.Deliveries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Delivery__Produc__59FA5E80");

            entity.HasOne(d => d.ReOrder).WithMany(p => p.Deliveries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Delivery__ReOrde__5AEE82B9");

            entity.HasOne(d => d.Reciever).WithMany(p => p.DeliveryRecievers).HasConstraintName("FK__Delivery__Reciev__324172E1");
        });

        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Develope__3214EC07E9F1E3DC");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07C6BB1988");

            entity.Property(e => e.Guid).IsFixedLength();
            entity.Property(e => e.UserName).HasDefaultValueSql("('penis')");
        });

        modelBuilder.Entity<EmployeeChange>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07DE6B0501");

            entity.HasOne(d => d.AffectedUserNavigation).WithMany(p => p.EmployeeChangeAffectedUserNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeChange__AffectedUser");

            entity.HasOne(d => d.ExecutingUserNavigation).WithMany(p => p.EmployeeChangeExecutingUserNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeChange__ExecutingUser");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genre__3214EC07D6972B0B");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Images__3214EC075F378477");

            entity.HasOne(d => d.Product).WithMany(p => p.Images)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductId");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC07A0AB5FCE");

            entity.Property(e => e.OrderNumber).IsFixedLength();

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__CustomerI__6B24EA82");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__PaymentMe__6C190EBB");
        });

        modelBuilder.Entity<OrderChange>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderCha__3214EC078A29F276");

            entity.HasOne(d => d.Employee).WithMany(p => p.OrderChanges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderChan__Emplo__42ACE4D4");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderChanges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderChan__Order__41B8C09B");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderPro__3214EC07D5428754");

            entity.Property(e => e.Price).HasDefaultValueSql("((0.00))");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderProd__Order__6FE99F9F");

            entity.HasOne(d => d.Products).WithMany(p => p.OrderProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderProd__Produ__6EF57B66");
        });

        modelBuilder.Entity<PasswordReset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Password__3214EC0712BDEC2A");

            entity.Property(e => e.Guid).IsFixedLength();

            entity.HasOne(d => d.Customer).WithMany(p => p.PasswordResets).HasConstraintName("fk_orders_customers");

            entity.HasOne(d => d.Employee).WithMany(p => p.PasswordResets).HasConstraintName("FK__PasswordR__Emplo__4D5F7D71");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentM__3214EC07ECBDE2E0");

            entity.HasOne(d => d.CreditCard).WithMany(p => p.PaymentMethods).HasConstraintName("FK__PaymentMe__Credi__619B8048");

            entity.HasOne(d => d.Debit).WithMany(p => p.PaymentMethods).HasConstraintName("FK__PaymentMe__Debit__628FA481");
        });

        modelBuilder.Entity<PriceHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PriceHis__3214EC0785014D26");

            entity.HasOne(d => d.Product).WithMany(p => p.PriceHistories).HasConstraintName("FK__PriceHist__Produ__075714DC");

            entity.HasOne(d => d.SetByNavigation).WithMany(p => p.PriceHistories).HasConstraintName("FK__PriceHist__SetBy__084B3915");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07F702BE50");

            entity.Property(e => e.DeveloperId).HasDefaultValueSql("((2))");
            entity.Property(e => e.GenreId).HasDefaultValueSql("((1))");
            entity.Property(e => e.ProductNumber).IsFixedLength();

            entity.HasOne(d => d.Developer).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Developer");

            entity.HasOne(d => d.Genre).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__GenreI__5D60DB10");

            entity.HasOne(d => d.ProductCategory).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Produc__52593CB8");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductC__3214EC07E4012265");
        });

        modelBuilder.Entity<ReOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReOrder__3214EC072BF0B82E");

            entity.Property(e => e.ReOrderNumber).IsFixedLength();

            entity.HasOne(d => d.Creator).WithMany(p => p.ReOrderCreators)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReOrder__Creator__5629CD9C");

            entity.HasOne(d => d.LastEditor).WithMany(p => p.ReOrderLastEditors)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReOrder__LastEdi__571DF1D5");

            entity.HasOne(d => d.Sender).WithMany(p => p.ReOrderSenders).HasConstraintName("FK__ReOrder__SenderI__1F2E9E6D");
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shopping__3214EC0763775FDA");

            entity.HasOne(d => d.Customer).WithOne(p => p.ShoppingCart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ShoppingCart_Customer");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Store__3214EC071D2164D9");
        });

        modelBuilder.Entity<TempEmployee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TempEmpl__3214EC074D237648");

            entity.Property(e => e.CreatorId).HasDefaultValueSql("((22))");
            entity.Property(e => e.Guid).IsFixedLength();

            entity.HasOne(d => d.Creator).WithMany(p => p.TempEmployees)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TempEmplo__Creat__2F9A1060");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
