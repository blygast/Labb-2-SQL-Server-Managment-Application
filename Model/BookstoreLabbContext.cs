using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BokhandelApp.Model;

public partial class BookstoreLabbContext : DbContext
{
    public BookstoreLabbContext()
    {
    }

    public BookstoreLabbContext(DbContextOptions<BookstoreLabbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookAuthor> BookAuthors { get; set; }

    public virtual DbSet<BookBalance> BookBalances { get; set; }

    public virtual DbSet<ButikFörsäljning> ButikFörsäljnings { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<TitlarPerForfattare> TitlarPerForfattares { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer("Data Source=localhost;Database=BookstoreLabb;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("pk_authors");

            entity.ToTable("authors");

            entity.Property(e => e.AuthorId).HasColumnName("authorId");
            entity.Property(e => e.DateOfBirth).HasColumnName("dateOfBirth");
            entity.Property(e => e.DeathDate).HasColumnName("deathDate");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("lastName");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Isbn).HasName("pk_books");

            entity.ToTable("books");

            entity.HasIndex(e => e.PublisherId, "ix_books_publisherId");

            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn");
            entity.Property(e => e.Format)
                .HasMaxLength(25)
                .HasColumnName("format");
            entity.Property(e => e.Language)
                .HasMaxLength(15)
                .HasColumnName("language");
            entity.Property(e => e.Pages).HasColumnName("pages");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.PublishDate).HasColumnName("publishDate");
            entity.Property(e => e.PublisherId).HasColumnName("publisherId");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .HasColumnName("title");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("fk_books_publishers");

            entity.HasMany(d => d.Categories).WithMany(p => p.Isbns)
                .UsingEntity<Dictionary<string, object>>(
                    "BookCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_bookCategories_categories"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("Isbn")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_bookCategories_books"),
                    j =>
                    {
                        j.HasKey("Isbn", "CategoryId").HasName("pk_bookCategories");
                        j.ToTable("bookCategories");
                        j.IndexerProperty<string>("Isbn")
                            .HasMaxLength(13)
                            .IsUnicode(false)
                            .IsFixedLength()
                            .HasColumnName("isbn");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("categoryId");
                    });
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(e => new { e.Isbn, e.AuthorId }).HasName("pk_bookAuthors");

            entity.ToTable("bookAuthors");

            entity.HasIndex(e => e.AuthorId, "ix_bookAuthors_authorId");

            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn");
            entity.Property(e => e.AuthorId).HasColumnName("authorId");
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .HasColumnName("role");

            entity.HasOne(d => d.Author).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bookAuthors_authors");

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.Isbn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bookAuthors_books");
        });

        modelBuilder.Entity<BookBalance>(entity =>
        {
            entity.HasKey(e => new { e.StoreId, e.Isbn }).HasName("pk_bookBalance");

            entity.ToTable("bookBalance");

            entity.HasIndex(e => e.Isbn, "ix_bookBalance_isbn");

            entity.Property(e => e.StoreId).HasColumnName("storeId");
            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn");
            entity.Property(e => e.AmountInStock).HasColumnName("amountInStock");

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.BookBalances)
                .HasForeignKey(d => d.Isbn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bookBalance_book");

            entity.HasOne(d => d.Store).WithMany(p => p.BookBalances)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bookBalance_store");
        });

        modelBuilder.Entity<ButikFörsäljning>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ButikFörsäljning");

            entity.Property(e => e.AntalOrdrar).HasColumnName("antalOrdrar");
            entity.Property(e => e.Butik)
                .HasMaxLength(60)
                .HasColumnName("butik");
            entity.Property(e => e.Omsättning)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("omsättning");
            entity.Property(e => e.SnittOrder)
                .HasColumnType("decimal(38, 6)")
                .HasColumnName("snittOrder");
            entity.Property(e => e.StoreId).HasColumnName("storeId");
            entity.Property(e => e.UnikaKunder).HasColumnName("unikaKunder");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("pk_categories");

            entity.ToTable("categories");

            entity.HasIndex(e => e.CategoryName, "uq_categories_name").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("categoryId");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(80)
                .HasColumnName("categoryName");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("pk_customers");

            entity.ToTable("customers");

            entity.HasIndex(e => e.Email, "uq_customers_email").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(80)
                .HasColumnName("lastName");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RegisteredAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("registeredAt");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("pk_orders");

            entity.ToTable("orders");

            entity.HasIndex(e => e.CustomerId, "ix_orders_customerId");

            entity.HasIndex(e => e.StoreId, "ix_orders_storeId");

            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.OrderDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("orderDate");
            entity.Property(e => e.StoreId).HasColumnName("storeId");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("totalAmount");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_customer");

            entity.HasOne(d => d.Store).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_store");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.Isbn }).HasName("pk_orderDetails");

            entity.ToTable("orderDetails");

            entity.HasIndex(e => e.Isbn, "ix_orderDetails_isbn");

            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn");
            entity.Property(e => e.BookPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("bookPrice");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.Isbn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orderDetails_book");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orderDetails_order");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("pk_publishers");

            entity.ToTable("publishers");

            entity.Property(e => e.PublisherId).HasColumnName("publisherId");
            entity.Property(e => e.Country)
                .HasMaxLength(60)
                .HasColumnName("country");
            entity.Property(e => e.PublisherName)
                .HasMaxLength(120)
                .HasColumnName("publisherName");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.StoreId).HasName("pk_stores");

            entity.ToTable("stores");

            entity.Property(e => e.StoreId).HasColumnName("storeId");
            entity.Property(e => e.Address)
                .HasMaxLength(120)
                .HasColumnName("address");
            entity.Property(e => e.StoreName)
                .HasMaxLength(60)
                .HasColumnName("storeName");
        });

        modelBuilder.Entity<TitlarPerForfattare>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TitlarPerForfattare");

            entity.Property(e => e.Lagervärde).HasMaxLength(4000);
            entity.Property(e => e.Namn).HasMaxLength(151);
            entity.Property(e => e.Titlar).HasMaxLength(15);
            entity.Property(e => e.Ålder).HasMaxLength(15);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
