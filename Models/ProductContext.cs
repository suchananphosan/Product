using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProductApp.Models;

public partial class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Productmaster> Productmasters { get; set; }
    public virtual DbSet<Producttype> Producttypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Productmaster>(entity =>
        {
            entity.HasKey(e => e.Productid).HasName("PK__PRODUCTM__34980AA22C919B6D");
            entity.ToTable("PRODUCTMASTER");

            entity.Property(e => e.Productid).HasMaxLength(20).IsUnicode(false).HasColumnName("PRODUCTID");
            entity.Property(e => e.Createdate).HasColumnType("datetime").HasColumnName("CREATEDATE");
            entity.Property(e => e.Createuser).HasMaxLength(20).IsUnicode(false).HasColumnName("CREATEUSER");
            entity.Property(e => e.Productname).HasMaxLength(20).IsUnicode(false).HasColumnName("PRODUCTNAME");
            entity.Property(e => e.Productstatus).HasColumnName("PRODUCTSTATUS");
            entity.Property(e => e.Producttypeid).HasMaxLength(5).IsUnicode(false).HasColumnName("PRODUCTTYPEID");
            entity.Property(e => e.Updatedate).HasColumnType("datetime").HasColumnName("UPDATEDATE");
            entity.Property(e => e.Updateuser).HasMaxLength(20).IsUnicode(false).HasColumnName("UPDATEUSER");
        });

        modelBuilder.Entity<Producttype>(entity =>
        {
            entity.HasKey(e => e.Producttypeid).HasName("PK__PRODUCTT__02B1F7956815F0B3");
            entity.ToTable("PRODUCTTYPE");

            entity.Property(e => e.Producttypeid).HasMaxLength(5).IsUnicode(false).HasColumnName("PRODUCTTYPEID");
            entity.Property(e => e.Producttypename).HasMaxLength(20).IsUnicode(false).HasColumnName("PRODUCTTYPENAME");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}