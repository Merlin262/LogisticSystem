using System;
using System.Collections.Generic;
using logisticsSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace logisticsSystem.Data;

public partial class LogisticsSystemContext : DbContext
{
    public LogisticsSystemContext()
    {
    }

    public LogisticsSystemContext(DbContextOptions<LogisticsSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Deduction> Deductions { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeWage> EmployeeWages { get; set; }

    public virtual DbSet<ItensShipped> ItensShippeds { get; set; }

    public virtual DbSet<ItensStock> ItensStocks { get; set; }

    public virtual DbSet<Maintenance> Maintenances { get; set; }

    public virtual DbSet<MaitenanceTruckPart> MaitenanceTruckParts { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Phone> Phones { get; set; }

    public virtual DbSet<Shipping> Shippings { get; set; }

    public virtual DbSet<ShippingPayment> ShippingPayments { get; set; }

    public virtual DbSet<Truck> Trucks { get; set; }

    public virtual DbSet<TruckDriver> TruckDrivers { get; set; }

    public virtual DbSet<TruckPart> TruckParts { get; set; }

    public virtual DbSet<WageDeduction> WageDeductions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=logisticsSystem;Trusted_Connection=true;encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Address__3214EC27D081713B");

            entity.ToTable("Address");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Complement)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Number)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Street)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Zipcode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ZIPCode");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.FkPersonId).HasName("PK__Client__25ADBC315E315C7F");

            entity.ToTable("Client");

            entity.Property(e => e.FkPersonId)
                .ValueGeneratedNever()
                .HasColumnName("fk_Person_ID");

            entity.HasOne(d => d.FkPerson).WithOne(p => p.Client)
                .HasForeignKey<Client>(d => d.FkPersonId)
                .HasConstraintName("FK__Client__fk_Perso__5165187F");
        });

        modelBuilder.Entity<Deduction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Deductio__3214EC2796C048E0");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.FkPersonId).HasName("PK__Employee__25ADBC3138D6935B");

            entity.ToTable("Employee");

            entity.Property(e => e.FkPersonId)
                .ValueGeneratedNever()
                .HasColumnName("fk_Person_ID");
            entity.Property(e => e.Position)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.FkPerson).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.FkPersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__fk_Per__5441852A");
        });

        modelBuilder.Entity<EmployeeWage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC27DBD1E70B");

            entity.ToTable("Employee_Wage");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ComissionPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Commission).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FkEmployeeId).HasColumnName("fk_Employee_ID");

            entity.HasOne(d => d.FkEmployee).WithMany(p => p.EmployeeWages)
                .HasForeignKey(d => d.FkEmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Employee___fk_Em__571DF1D5");
        });

        modelBuilder.Entity<ItensShipped>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ItensShi__3214EC27D4B5C92B");

            entity.ToTable("ItensShipped");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.FkItensStockId).HasColumnName("fk_ItensStock_ID");
            entity.Property(e => e.FkShippingId).HasColumnName("fk_Shipping_ID");

            entity.HasOne(d => d.FkItensStock).WithMany(p => p.ItensShippeds)
                .HasForeignKey(d => d.FkItensStockId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__ItensShip__fk_It__75A278F5");

            entity.HasOne(d => d.FkShipping).WithMany(p => p.ItensShippeds)
                .HasForeignKey(d => d.FkShippingId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ItensShip__fk_Sh__76969D2E");
        });

        modelBuilder.Entity<ItensStock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ItensSto__3214EC27A65C60B2");

            entity.ToTable("ItensStock");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Weight).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Maintenance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Maintena__3214EC27375E2753");

            entity.ToTable("Maintenance");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.FkEmployee).HasColumnName("fk_Employee");
            entity.Property(e => e.FkTruckChassis).HasColumnName("fk_Truck_Chassis");

            entity.HasOne(d => d.FkEmployeeNavigation).WithMany(p => p.Maintenances)
                .HasForeignKey(d => d.FkEmployee)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Maintenan__fk_Em__5DCAEF64");

            entity.HasOne(d => d.FkTruckChassisNavigation).WithMany(p => p.Maintenances)
                .HasForeignKey(d => d.FkTruckChassis)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Maintenan__fk_Tr__5EBF139D");
        });

        modelBuilder.Entity<MaitenanceTruckPart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Maitenan__3214EC27E2CCE34F");

            entity.ToTable("MaitenanceTruckPart");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.FkMaintenanceId).HasColumnName("fk_Maintenance_ID");
            entity.Property(e => e.FkTruckPartId).HasColumnName("fk_TruckPart_ID");

            entity.HasOne(d => d.FkMaintenance).WithMany(p => p.MaitenanceTruckParts)
                .HasForeignKey(d => d.FkMaintenanceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Maitenanc__fk_Ma__72C60C4A");

            entity.HasOne(d => d.FkTruckPart).WithMany(p => p.MaitenanceTruckParts)
                .HasForeignKey(d => d.FkTruckPartId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Maitenanc__fk_Tr__71D1E811");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Person__3214EC27CBE5F595");

            entity.ToTable("Person");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FkAddressId).HasColumnName("fk_Address_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.FkAddress).WithMany(p => p.People)
                .HasForeignKey(d => d.FkAddressId)
                .HasConstraintName("FK__Person__fk_Addre__4BAC3F29");
        });

        modelBuilder.Entity<Phone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Phone__3214EC2702FD4DC7");

            entity.ToTable("Phone");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.AreaCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.FkPersonId).HasColumnName("fk_Person_ID");
            entity.Property(e => e.Number)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.FkPerson).WithMany(p => p.Phones)
                .HasForeignKey(d => d.FkPersonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Phone__fk_Person__4E88ABD4");
        });

        modelBuilder.Entity<Shipping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shipping__3214EC27FFC34A2A");

            entity.ToTable("Shipping");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.DistanceKm).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FkAddressId).HasColumnName("fk_Address_ID");
            entity.Property(e => e.FkClientId).HasColumnName("fk_Client_ID");
            entity.Property(e => e.FkEmployeeId).HasColumnName("fk_Employee_ID");
            entity.Property(e => e.FkTruckId).HasColumnName("fk_Truck_ID");
            entity.Property(e => e.ShippingPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalWeight).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.FkAddress).WithMany(p => p.Shippings)
                .HasForeignKey(d => d.FkAddressId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Shipping__fk_Add__6754599E");

            entity.HasOne(d => d.FkClient).WithMany(p => p.Shippings)
                .HasForeignKey(d => d.FkClientId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Shipping__fk_Cli__656C112C");

            entity.HasOne(d => d.FkEmployee).WithMany(p => p.Shippings)
                .HasForeignKey(d => d.FkEmployeeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Shipping__fk_Emp__66603565");

            entity.HasOne(d => d.FkTruck).WithMany(p => p.Shippings)
                .HasForeignKey(d => d.FkTruckId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Shipping__fk_Tru__68487DD7");
        });

        modelBuilder.Entity<ShippingPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shipping__3214EC2777643CB6");

            entity.ToTable("ShippingPayment");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.FkShippingId).HasColumnName("fk_Shipping_ID");

            entity.HasOne(d => d.FkShipping).WithMany(p => p.ShippingPayments)
                .HasForeignKey(d => d.FkShippingId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ShippingP__fk_Sh__6B24EA82");
        });

        modelBuilder.Entity<Truck>(entity =>
        {
            entity.HasKey(e => e.Chassis).HasName("PK__Truck__9C351ECE75DBB27B");

            entity.ToTable("Truck");

            entity.Property(e => e.Chassis).ValueGeneratedNever();
            entity.Property(e => e.Color)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Model)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TruckDriver>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TruckDri__3214EC273F74401A");

            entity.ToTable("TruckDriver");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.FkEmployeeId).HasColumnName("fk_Employee_ID");
            entity.Property(e => e.FkTruckChassis).HasColumnName("fk_Truck_Chassis");

            entity.HasOne(d => d.FkEmployee).WithMany(p => p.TruckDrivers)
                .HasForeignKey(d => d.FkEmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TruckDriv__fk_Em__6EF57B66");

            entity.HasOne(d => d.FkTruckChassisNavigation).WithMany(p => p.TruckDrivers)
                .HasForeignKey(d => d.FkTruckChassis)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TruckDriv__fk_Tr__6E01572D");
        });

        modelBuilder.Entity<TruckPart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TruckPar__3214EC27D906E059");

            entity.ToTable("TruckPart");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WageDeduction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WageDedu__3214EC2742F0C8EF");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.FkDeductionsId).HasColumnName("fk_Deductions_ID");
            entity.Property(e => e.FkWageId).HasColumnName("fk_Wage_ID");

            entity.HasOne(d => d.FkDeductions).WithMany(p => p.WageDeductions)
                .HasForeignKey(d => d.FkDeductionsId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__WageDeduc__fk_De__797309D9");

            entity.HasOne(d => d.FkWage).WithMany(p => p.WageDeductions)
                .HasForeignKey(d => d.FkWageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__WageDeduc__fk_Wa__7A672E12");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
