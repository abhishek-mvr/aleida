using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Aleida1.Models
{
    public partial class DataServerContext : DbContext
    {
        public virtual DbSet<Pcdetails> Pcdetails { get; set; }
        public virtual DbSet<RawData> RawData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=DataServer;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pcdetails>(entity =>
            {
                entity.ToTable("PCDetails");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IpAddress).HasColumnType("nchar(20)");

                entity.Property(e => e.Name).HasColumnType("nchar(20)");
            });

            modelBuilder.Entity<RawData>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Failure).HasColumnName("failure");

                entity.Property(e => e.Lanip)
                    .IsRequired()
                    .HasColumnName("lanip")
                    .HasColumnType("nchar(20)");

                entity.Property(e => e.Port).HasColumnName("port");

                entity.Property(e => e.Time).HasColumnName("time");

                entity.Property(e => e.Wanip)
                    .IsRequired()
                    .HasColumnName("wanip")
                    .HasColumnType("nchar(20)");
            });
        }
    }
}
