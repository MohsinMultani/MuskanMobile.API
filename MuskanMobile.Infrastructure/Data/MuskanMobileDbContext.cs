using Microsoft.EntityFrameworkCore;
using MuskanMobile.Domain.Common;
using MuskanMobile.Domain.Entities;
using System;

namespace MuskanMobile.Infrastructure.Data;

public partial class MuskanMobileDbContext : DbContext
{
    public MuskanMobileDbContext()
    {
    }

    public MuskanMobileDbContext(DbContextOptions<MuskanMobileDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    //public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<PurchaseItem> PurchaseItems { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<SalesItem> SalesItems { get; set; }

    public virtual DbSet<SalesOrder> SalesOrders { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<TaxRate> TaxRates { get; set; }

    public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=MuskanMobileDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var entity = modelBuilder.Entity(entityType.ClrType);

                // CreatedDate - required, database default
                entity.Property<DateTime>("CreatedDate")
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired(true)
                    .ValueGeneratedOnAdd();

                // ModifiedDate - optional, no default
                entity.Property<DateTime?>("ModifiedDate")
                    .IsRequired(false)
                    .ValueGeneratedOnUpdate();
            }
        }



        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B95548633");

            //entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
        });

        //modelBuilder.Entity<Customer>(entity =>
        //{
        //    entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D8CCBA37C1");

        //    //entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
        //});

        //modelBuilder.Entity<Payment>(entity =>
        //{
        //    entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A3889FF32A9");

        //    entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");

        //    entity.HasOne(d => d.SalesOrder).WithMany(p => p.Payments)
        //        .OnDelete(DeleteBehavior.ClientSetNull)
        //        .HasConstraintName("FK__Payments__SalesO__75A278F5");
        //});

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CDF1A2394D");

           // entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Catego__76969D2E");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products).HasConstraintName("FK__Products__Suppli__778AC167");

            entity.HasOne(d => d.TaxRate).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__TaxRat__787EE5A0");
        });

        modelBuilder.Entity<PurchaseItem>(entity =>
        {
            entity.HasKey(e => e.PurchaseItemId).HasName("PK__Purchase__B48BB68781277BAA");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseI__Produ__797309D9");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseI__Purch__7A672E12");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderId).HasName("PK__Purchase__036BACA440D6311C");

            entity.Property(e => e.PurchaseDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Suppl__7B5B524B");
        });

        modelBuilder.Entity<SalesItem>(entity =>
        {
            entity.HasKey(e => e.SalesItemId).HasName("PK__SalesIte__B97422C17E3D9C03");

            entity.HasOne(d => d.Product).WithMany(p => p.SalesItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalesItem__Produ__7C4F7684");

            entity.HasOne(d => d.SalesOrder).WithMany(p => p.SalesItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalesItem__Sales__7D439ABD");
        });

        modelBuilder.Entity<SalesOrder>(entity =>
        {
            entity.HasKey(e => e.SalesOrderId).HasName("PK__SalesOrd__B14003E2052517FE");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.SalesOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalesOrde__Custo__7E37BEF6");
        });


        modelBuilder.Entity<Supplier>(entity =>
        {
            
        });


        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Suppliers", "Muskan");
            //entity.HasKey(e => e.SupplierId);

            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B4A5333DE8");

            //entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            entity.Property(e => e.ModifiedDate)
            .HasColumnType("datetime2")
            .IsRequired(false);
        });

        modelBuilder.Entity<TaxRate>(entity =>
        {
            entity.ToTable("TaxRates", "Muskan");
            // Primary Key
            entity.HasKey(e => e.TaxRateId).HasName("PK__TaxRates__B114CEC1B5184191");

            // Properties
            entity.Property(e => e.TaxRateId)
                .HasColumnName("TaxRateId")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.TaxName)
                .HasColumnName("TaxName")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Rate)  // or Percentage if you kept that name
                .HasColumnName("Rate")    // match your column name
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("Description")
                .HasMaxLength(500)
                .IsRequired(false);

            entity.Property(e => e.IsActive)
                .HasColumnName("IsActive")
                .HasDefaultValue(true)
                .IsRequired();

            // Audit fields (from BaseEntity)
            entity.Property(e => e.CreatedDate)
                .HasColumnName("CreatedDate")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            entity.Property(e => e.ModifiedDate)
                .HasColumnName("ModifiedDate")
                .HasColumnType("datetime2")
                .IsRequired(false);

            // Relationships (if any)
            entity.HasMany(e => e.Products)
                .WithOne(p => p.TaxRate)
                .HasForeignKey(p => p.TaxRateId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers", "Muskan");
            entity.HasKey(e => e.CustomerId);

            entity.Property(e => e.CustomerName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Phone)
                .HasMaxLength(20);

            entity.Property(e => e.Email)
                .HasMaxLength(100);

            entity.Property(e => e.Address)
                .HasMaxLength(200);

            entity.Property(e => e.City)
                .HasMaxLength(50);

            entity.Property(e => e.State)
                .HasMaxLength(50);

            entity.Property(e => e.ZipCode)
                .HasMaxLength(20);

            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasDefaultValue("India");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    //public MuskanMobileDbContext(DbContextOptions<MuskanMobileDbContext> options) : base(options) { }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.HasDefaultSchema("Muskan");

    //    modelBuilder.Entity<Product>()
    //        .HasKey(p => p.ProductId);

    //    modelBuilder.Entity<Category>()
    //        .HasKey(c => c.CategoryId);

    //    modelBuilder.Entity<Supplier>()
    //        .HasKey(s => s.SupplierId);

    //    modelBuilder.Entity<TaxRate>()
    //        .HasKey(t => t.TaxRateId);

    //    modelBuilder.Entity<Product>()
    //        .HasOne(p => p.Category)
    //        .WithMany(c => c.Products)
    //        .HasForeignKey(p => p.CategoryId);

    //    modelBuilder.Entity<Product>()
    //        .HasOne(p => p.Supplier)
    //        .WithMany(s => s.Products)
    //        .HasForeignKey(p => p.SupplierId);

    //    modelBuilder.Entity<Product>()
    //        .HasOne(p => p.TaxRate)
    //        .WithMany(t => t.Products)
    //        .HasForeignKey(p => p.TaxRateId);
    //}

    //public DbSet<Product> Products => Set<Product>();
    //public DbSet<Category> Categories => Set<Category>();
    //public DbSet<Order> Orders => Set<Order>();
    //public DbSet<User> Users => Set<User>();
}

