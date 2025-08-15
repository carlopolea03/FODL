﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FODLSystem.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FODLSystem.Models
{
    public class FODLSystemContext : DbContext
    {
        public FODLSystemContext(DbContextOptions<FODLSystemContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Department> Departments { get; set; }

        public DbSet<NoSeries> NoSeries { get; set; }

        public DbSet<Item> Items { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<LubeTruck> LubeTrucks { get; set; }
        public DbSet<Dispenser> Dispensers { get; set; }
        public DbSet<Location> Locations { get; set; }
        
        

        public DbSet<FuelOil> FuelOils { get; set; }
        public DbSet<FuelOilDetail> FuelOilDetails { get; set; }
        public DbSet<FuelOilSubDetail> FuelOilSubDetails { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<SynchronizeInformation> SynchronizeInformations { get; set; }
        public DbSet<Driver> Drivers { get; set; }

        public DbSet<BSmartShift> BSmartShifts { get; set; }
        public DbSet<BSmartEquipment> BSmartEquipments { get; set; }
        public DbSet<BSmartItem> BSmartItems { get; set; }
        public DbSet<BSmartError> BSmartErrors { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
               .HasIndex(p => new { p.Username, p.Status })
               .IsUnique();

            modelBuilder.Entity<Department>()
            .Property(e => e.ID).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<Item>()
                .Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<Driver>()
                .Property(e => e.ID).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<Dispenser>()
                .Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<Component>()
                .Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<Equipment>()
                .Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<Driver>()
                .Property(e => e.ID).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<LubeTruck>()
                .Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<Location>()
                .Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            //modelBuilder.Entity<LubeTruck>()
            //  .HasIndex(p => new { p.No, p.Status })
            //  .IsUnique();

            //modelBuilder.Entity<Driver>()
            //   .HasIndex(p => new { p.IdNumber })
            //   .IsUnique();

            //modelBuilder.Entity<FuelOil>()
            // .HasIndex(p => new { p.TransactionDate, p.Shift, p.Status, p.LubeTruckId, p.DispenserId })
            // .IsUnique();

            modelBuilder.Entity<Company>().HasData(
               new { ID = 1, Code = "SMPC", Name = "Semirara Mining and Power Corporation", Status = "Active" }
            );

           modelBuilder.Entity<Department>().HasData(
               new { ID = 1, Code = "NA", Name = "NOTSET", Status = "Deleted", CompanyId = 1 }
           );

           modelBuilder.Entity<Role>().HasData(
                new { Id = 1, Name = "Admin", Status = "Active" },
                new { Id = 2, Name = "User", Status = "Active" }
           );

           modelBuilder.Entity<User>().HasData(
               new { Id = 1,Username = "jrventura", RoleId = 1,Password = "",FirstName = "jrventura", LastName = "jrventura", Status = "1", Email = "jrventura@semirarampc.com", DepartmentId = 1, Name = "jrventura", Domain = "SMCDACON", CompanyAccess = "1"}
           );

            //modelBuilder.Entity<FuelOil>()
            //    .HasIndex(r => r.ReferenceNo)
            //     .IsUnique();
            //modelBuilder.Entity<LubeTruck>().HasData(
            //    new { Id = 1, No = "na", OldId = "0", Description = "N/A", Status = "Default"}
            //);
            // modelBuilder.Entity<Dispenser>().HasData(
            //    new { Id = 1, No = "na", Name = "N/A", Status = "Default" }
            //);
            // modelBuilder.Entity<Location>().HasData(
            //    new { Id = 1, No = "na", List = "N/A", OfficeCode = "000", Status = "Default" }
            //);
            //  modelBuilder.Entity<Component>().HasData(
            //    new { Id = 1, Code = "N/A", Status = "Default", DateModified = DateTime.Now }
            //);

            // modelBuilder.Entity<Driver>().HasData(
            //   new { ID = 1, IdNumber = "00000", Name = "N/A",Position = "N/A", Status = "Enabled", DateModified = new DateTime(1900,01,01) }
            //);
        }

        internal object Include(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}
