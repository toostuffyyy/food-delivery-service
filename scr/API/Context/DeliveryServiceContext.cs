using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using api.Entities;

namespace api.Context;

public partial class DeliveryServiceContext : DbContext
{
    public DeliveryServiceContext()
    {
    }

    public DeliveryServiceContext(DbContextOptions<DeliveryServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<CategoryProduct> CategoryProducts { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }

    public virtual DbSet<EmployeeSalary> EmployeeSalaries { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

    public virtual DbSet<Packaging> Packagings { get; set; }

    public virtual DbSet<Parameter> Parameters { get; set; }

    public virtual DbSet<ParameterProduct> ParameterProducts { get; set; }

    public virtual DbSet<Pay> Pays { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<StatusPay> StatusPays { get; set; }

    public virtual DbSet<TypePay> TypePays { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionString:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("Address");

            entity.Property(e => e.Comment).HasMaxLength(150);
            entity.Property(e => e.Street).HasMaxLength(50);

            entity.HasOne(d => d.Client).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_Address_Client");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("Brand");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<CategoryProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CatogoriesProducts");

            entity.ToTable("CategoryProduct");

            entity.Property(e => e.ImagePath).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.ParentCategoryProduct).WithMany(p => p.InverseParentCategoryProduct)
                .HasForeignKey(d => d.ParentCategoryProductId)
                .HasConstraintName("FK_CategoryProduct_CategoryProduct");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Client");

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.ImagePath).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(250);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.RefreshToken).HasMaxLength(250);
            entity.Property(e => e.RefreshTokenExp).HasColumnType("datetime");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Manager");

            entity.ToTable("Employee");

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.ImagePath).HasMaxLength(150);
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(250);
            entity.Property(e => e.Patronymic).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.RefreshToken).HasMaxLength(250);
            entity.Property(e => e.RefreshTokenExp).HasColumnType("datetime");
            entity.Property(e => e.Surname).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Manager_ManagerRole");
        });

        modelBuilder.Entity<EmployeeRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ManagerRole");

            entity.ToTable("EmployeeRole");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<EmployeeSalary>(entity =>
        {
            entity.HasKey(e => e.EmployeeRole).HasName("PK_EmployeeRate");

            entity.ToTable("EmployeeSalary");

            entity.Property(e => e.EmployeeRole).ValueGeneratedNever();
            entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.EmployeeRoleNavigation).WithOne(p => p.EmployeeSalary)
                .HasForeignKey<EmployeeSalary>(d => d.EmployeeRole)
                .HasConstraintName("FK_EmployeeRate_EmployeeRole");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.Comment).HasMaxLength(150);
            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.MinSum).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceAssembly).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceDelivery).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            entity.Property(e => e.Street).HasMaxLength(50);

            entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_Order_Client1");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Order_Status");

            entity.HasMany(d => d.Shifts).WithMany(p => p.Orders)
                .UsingEntity<Dictionary<string, object>>(
                    "ShiftEmployee",
                    r => r.HasOne<Shift>().WithMany()
                        .HasForeignKey("ShiftId")
                        .HasConstraintName("FK_OrderEmployee_Shift"),
                    l => l.HasOne<Order>().WithMany()
                        .HasForeignKey("OrderId")
                        .HasConstraintName("FK_OrderEmployee_Order"),
                    j =>
                    {
                        j.HasKey("OrderId", "ShiftId").HasName("PK_OrderEmployee");
                        j.ToTable("ShiftEmployee");
                    });
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId });

            entity.ToTable("OrderItem");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderItem_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_OrderItem_Product");
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_OrderStatusHistory_1");

            entity.ToTable("OrderStatusHistory");

            entity.Property(e => e.DateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderStatusHistory_Order");

            entity.HasOne(d => d.Status).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStatusHistory_Status");
        });

        modelBuilder.Entity<Packaging>(entity =>
        {
            entity.ToTable("Packaging");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Parameter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ParameterProduct");

            entity.ToTable("Parameter");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<ParameterProduct>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.ParameterId }).HasName("PK_ProductParameterProduct");

            entity.ToTable("ParameterProduct");

            entity.Property(e => e.Value).HasMaxLength(150);

            entity.HasOne(d => d.Parameter).WithMany(p => p.ParameterProducts)
                .HasForeignKey(d => d.ParameterId)
                .HasConstraintName("FK_ProductParameterProduct_ParameterProduct");

            entity.HasOne(d => d.Product).WithMany(p => p.ParameterProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductParameterProduct_OptionProduct");
        });

        modelBuilder.Entity<Pay>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("Pay");

            entity.Property(e => e.OrderId).ValueGeneratedNever();
            entity.Property(e => e.DateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithOne(p => p.Pay)
                .HasForeignKey<Pay>(d => d.OrderId)
                .HasConstraintName("FK_Pay_Order");

            entity.HasOne(d => d.StatusPay).WithMany(p => p.Pays)
                .HasForeignKey(d => d.StatusPayId)
                .HasConstraintName("FK_Pay_StatusPay");

            entity.HasOne(d => d.TypePay).WithMany(p => p.Pays)
                .HasForeignKey(d => d.TypePayId)
                .HasConstraintName("FK_Pay_TypePay");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_OptionProduct");

            entity.ToTable("Product");

            entity.Property(e => e.Composition).HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StorageConditions).HasMaxLength(100);

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK_Product_Brand");

            entity.HasOne(d => d.CategoryProduct).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_CategoryProduct");

            entity.HasOne(d => d.Packaging).WithMany(p => p.Products)
                .HasForeignKey(d => d.PackagingId)
                .HasConstraintName("FK_Product_Packaging");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Products)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OptionProduct_Promotion");

            entity.HasOne(d => d.Unit).WithMany(p => p.Products)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OptionProduct_Unit");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.ToTable("ProductImage");

            entity.Property(e => e.ImagePath).HasMaxLength(150);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductImage_OptionProduct");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Promotions");

            entity.ToTable("Promotion");

            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK_Reviews");

            entity.ToTable("Review");

            entity.Property(e => e.OrderId).ValueGeneratedNever();
            entity.Property(e => e.Comment).HasMaxLength(250);
            entity.Property(e => e.CreateDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_Reviews_Client");

            entity.HasOne(d => d.Order).WithOne(p => p.Review)
                .HasForeignKey<Review>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_Order1");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.ToTable("Shift");

            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.Shifts)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Shift_Employee");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<StatusPay>(entity =>
        {
            entity.ToTable("StatusPay");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TypePay>(entity =>
        {
            entity.ToTable("TypePay");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.ToTable("Unit");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
