using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Zoolirante.Models;
using Zoolirante.ViewModels;

namespace Zoolirante.Data;

public partial class ZooliranteContext : DbContext
{
    public ZooliranteContext()
    {
    }

    public ZooliranteContext(DbContextOptions<ZooliranteContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Animal> Animals { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventRollCall> EventRollCalls { get; set; }

    public virtual DbSet<FavouriteAnimal> FavouriteAnimals { get; set; }

    public virtual DbSet<MerchInOrder> MerchInOrders { get; set; }

    public virtual DbSet<Merchandise> Merchandises { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<PurchaseHistory> PurchaseHistories { get; set; }

    public virtual DbSet<Receipt> Receipts { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Species> Species { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Visitor> Visitors { get; set; }

    public virtual DbSet<VisitorMerchOrder> VisitorMerchOrders { get; set; }

    public virtual DbSet<ZooKeeper> ZooKeepers { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-DFL37JI\\SQLEXPRESS;Initial Catalog=Zoolirante;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__AD05008679C2C24F");

            entity.ToTable("Admin");

            entity.Property(e => e.AdminId)
                .ValueGeneratedNever()
                .HasColumnName("adminID");

            entity.HasOne(d => d.AdminNavigation).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admin_Staff");
        });

        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.AnimalId).HasName("PK__Animals__68745631C80680D0");

            entity.HasIndex(e => e.SpeciesId, "IX_Animals_speciesID");

            entity.Property(e => e.AnimalId).HasColumnName("animalID");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.AnimalImage)
                .HasMaxLength(500)
                .HasColumnName("animalImage");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.SpeciesId).HasColumnName("speciesID");

            entity.HasOne(d => d.Species).WithMany(p => p.Animals)
                .HasForeignKey(d => d.SpeciesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Animals_Species");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__2DC7BD69930FA981");

            entity.Property(e => e.EventId).HasColumnName("eventID");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
        });

        modelBuilder.Entity<EventRollCall>(entity =>
        {
            entity.HasKey(e => e.EventRollCallId).HasName("PK__EventRol__CB1E5E29377C26EC");

            entity.ToTable("EventRollCall");

            entity.HasIndex(e => e.EventId, "IX_RollCall_eventID");

            entity.HasIndex(e => new { e.EventId, e.RollDate, e.RollTime }, "UQ_RollCall").IsUnique();

            entity.Property(e => e.EventRollCallId).HasColumnName("eventRollCallID");
            entity.Property(e => e.EventId).HasColumnName("eventID");
            entity.Property(e => e.RollDate).HasColumnName("rollDate");
            entity.Property(e => e.RollTime).HasColumnName("rollTime");
            entity.Property(e => e.ZookeeperId).HasColumnName("zookeeperID");

            entity.HasOne(d => d.Event).WithMany(p => p.EventRollCalls)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RollCall_Evt");

            entity.HasOne(d => d.Zookeeper).WithMany(p => p.EventRollCalls)
                .HasForeignKey(d => d.ZookeeperId)
                .HasConstraintName("FK_RollCall_ZK");
        });

        modelBuilder.Entity<FavouriteAnimal>(entity =>
        {
            entity.HasKey(e => e.FavAnimalsId).HasName("PK__Favourit__283318836AFB5B3A");

            entity.HasIndex(e => e.AnimalId, "IX_Fav_Animal");

            entity.HasIndex(e => e.VisitorId, "IX_Fav_Visitor");

            entity.HasIndex(e => new { e.VisitorId, e.AnimalId }, "UQ_Fav_Visitor_Animal").IsUnique();

            entity.Property(e => e.FavAnimalsId).HasColumnName("favAnimalsID");
            entity.Property(e => e.AnimalId).HasColumnName("animalID");
            entity.Property(e => e.VisitorId).HasColumnName("visitorID");

            entity.HasOne(d => d.Animal).WithMany(p => p.FavouriteAnimals)
                .HasForeignKey(d => d.AnimalId)
                .HasConstraintName("FK_Fav_A");

            entity.HasOne(d => d.Visitor).WithMany(p => p.FavouriteAnimals)
                .HasForeignKey(d => d.VisitorId)
                .HasConstraintName("FK_Fav_V");
        });

        modelBuilder.Entity<MerchInOrder>(entity =>
        {
            entity.HasKey(e => new { e.OrderNumber, e.ItemId }).HasName("PK__MerchInO__D7FC001A3E8F610F");

            entity.ToTable("MerchInOrder");

            entity.Property(e => e.OrderNumber).HasColumnName("orderNumber");
            entity.Property(e => e.ItemId).HasColumnName("itemID");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("unitPrice");

            entity.HasOne(d => d.Item).WithMany(p => p.MerchInOrders)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MIO_Item");

            entity.HasOne(d => d.OrderNumberNavigation).WithMany(p => p.MerchInOrders)
                .HasForeignKey(d => d.OrderNumber)
                .HasConstraintName("FK_MIO_Order");
        });

        modelBuilder.Entity<Merchandise>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Merchand__56A1284A78AB4964");

            entity.ToTable("Merchandise");

            entity.Property(e => e.ItemId).HasColumnName("itemID");
            entity.Property(e => e.ItemCost)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("itemCost");
            entity.Property(e => e.ItemDescription)
                .HasMaxLength(2000)
                .HasColumnName("itemDescription");
            entity.Property(e => e.ItemImage)
                .HasMaxLength(500)
                .HasColumnName("itemImage");
            entity.Property(e => e.ItemName)
                .HasMaxLength(200)
                .HasColumnName("itemName");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__People__EC7D7D6DC771A9E0");

            entity.HasIndex(e => e.Email, "UQ__People__AB6E6164B6D68B49").IsUnique();

            entity.Property(e => e.PersonId).HasColumnName("personID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("lastName");
        });

        modelBuilder.Entity<PurchaseHistory>(entity =>
        {
            entity.HasKey(e => e.PurchaseHistoryId).HasName("PK__Purchase__D3C755D9A5DFDEC0");

            entity.ToTable("PurchaseHistory");

            entity.Property(e => e.PurchaseHistoryId)
                .ValueGeneratedNever()
                .HasColumnName("purchaseHistoryID");

            entity.HasOne(d => d.PurchaseHistoryNavigation).WithOne(p => p.PurchaseHistory)
                .HasForeignKey<PurchaseHistory>(d => d.PurchaseHistoryId)
                .HasConstraintName("FK_PH_Visitor");
        });

        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.HasKey(e => e.ReceiptId).HasName("PK__Receipts__CAA7E89870A32382");

            entity.HasIndex(e => e.PurchaseHistoryId, "IX_Receipts_ph");

            entity.Property(e => e.ReceiptId).HasColumnName("receiptID");
            entity.Property(e => e.DateTime)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("dateTime");
            entity.Property(e => e.PurchaseHistoryId).HasColumnName("purchaseHistoryID");
            entity.Property(e => e.ReceiptDetails).HasColumnName("receiptDetails");

            entity.HasOne(d => d.PurchaseHistory).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.PurchaseHistoryId)
                .HasConstraintName("FK_Receipt_PH");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__CD98460A0B23B498");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__B19478616E350041").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("roleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<Species>(entity =>
        {
            entity.HasKey(e => e.SpeciesId).HasName("PK__Species__FB702AB806DBF5F3");

            entity.HasIndex(e => e.Name, "UQ__Species__72E12F1BA62EBE41").IsUnique();

            entity.Property(e => e.SpeciesId).HasColumnName("speciesID");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.SpeciesImage)
                .HasMaxLength(500)
                .HasColumnName("speciesImage");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staff__6465E19E9856C213");

            entity.HasIndex(e => e.RoleId, "IX_Staff_roleID");

            entity.Property(e => e.StaffId)
                .ValueGeneratedNever()
                .HasColumnName("staffID");
            entity.Property(e => e.Mobile)
                .HasMaxLength(50)
                .HasColumnName("mobile");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Photo)
                .HasMaxLength(255)
                .HasColumnName("photo");
            entity.Property(e => e.RoleId).HasColumnName("roleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Staff)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staff_Roles");

            entity.HasOne(d => d.StaffNavigation).WithOne(p => p.Staff)
                .HasForeignKey<Staff>(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staff_People");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Tickets__3333C670CBCCC000");

            entity.HasIndex(e => e.VisitorId, "IX_Tickets_visitorID");

            entity.Property(e => e.TicketId).HasColumnName("ticketID");
            entity.Property(e => e.DateOfEntry).HasColumnName("dateOfEntry");
            entity.Property(e => e.DatePaid)
                .HasColumnType("datetime")
                .HasColumnName("datePaid");
            entity.Property(e => e.VisitorId).HasColumnName("visitorID");

            entity.HasOne(d => d.Visitor).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.VisitorId)
                .HasConstraintName("FK_Tickets_Visitor");
        });

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.HasKey(e => e.VisitorId).HasName("PK__Visitors__7D47DFA2A8452676");

            entity.HasIndex(e => e.Username, "UQ__Visitors__F3DBC572BC6BD2B0").IsUnique();

            entity.Property(e => e.VisitorId)
                .ValueGeneratedNever()
                .HasColumnName("visitorID");
            entity.Property(e => e.Contact)
                .HasMaxLength(100)
                .HasColumnName("contact");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("passwordHash");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(100)
                .HasColumnName("paymentMethod");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.VisitorNavigation).WithOne(p => p.Visitor)
                .HasForeignKey<Visitor>(d => d.VisitorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Visitors_People");
        });

        modelBuilder.Entity<VisitorMerchOrder>(entity =>
        {
            entity.HasKey(e => e.OrderNumber).HasName("PK__VisitorM__6296129EB4CD18DF");

            entity.ToTable("VisitorMerchOrder");

            entity.HasIndex(e => e.VisitorId, "IX_Order_visitorID");

            entity.Property(e => e.OrderNumber).HasColumnName("orderNumber");
            entity.Property(e => e.DatePaid)
                .HasColumnType("datetime")
                .HasColumnName("datePaid");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("orderDate");
            entity.Property(e => e.VisitorId).HasColumnName("visitorID");

            entity.HasOne(d => d.Visitor).WithMany(p => p.VisitorMerchOrders)
                .HasForeignKey(d => d.VisitorId)
                .HasConstraintName("FK_Order_Visitor");
        });

        modelBuilder.Entity<ZooKeeper>(entity =>
        {
            entity.HasKey(e => e.ZookeeperId).HasName("PK__ZooKeepe__33C5B953E6E020E6");

            entity.ToTable("ZooKeeper");

            entity.Property(e => e.ZookeeperId)
                .ValueGeneratedNever()
                .HasColumnName("zookeeperID");
            entity.Property(e => e.Description).HasColumnName("description");

            entity.HasOne(d => d.Zookeeper).WithOne(p => p.ZooKeeper)
                .HasForeignKey<ZooKeeper>(d => d.ZookeeperId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ZK_Staff");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<Zoolirante.ViewModels.DefaultViewModel> DefaultViewModel { get; set; } = default!;
}
