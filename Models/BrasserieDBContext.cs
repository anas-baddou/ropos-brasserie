using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace webAPIBrasserie.Models
{
    public partial class BrasserieDBContext : DbContext
    {
        public BrasserieDBContext()
        {
        }

        public BrasserieDBContext(DbContextOptions<BrasserieDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Biere> Bieres { get; set; } = null!;
        public virtual DbSet<Brasseur> Brasseurs { get; set; } = null!;
        public virtual DbSet<Grossiste> Grossistes { get; set; } = null!;
        public virtual DbSet<Vente> Ventes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Biere>(entity =>
            {
                entity.ToTable("Biere");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BrasseurId).HasColumnName("brasseur_id");

                entity.Property(e => e.DegresAlcool).HasColumnName("degres_alcool");

                entity.Property(e => e.Nom)
                    .HasMaxLength(255)
                    .HasColumnName("nom");

                entity.Property(e => e.Prix).HasColumnName("prix");

                entity.Property(e => e.Quantite).HasColumnName("quantite");

                entity.HasOne(d => d.Brasseur)
                    .WithMany(p => p.Bieres)
                    .HasForeignKey(d => d.BrasseurId)
                    .HasConstraintName("FK__Biere__brasseur___398D8EEE");
            });

            modelBuilder.Entity<Brasseur>(entity =>
            {
                entity.ToTable("Brasseur");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Adresse)
                    .HasMaxLength(255)
                    .HasColumnName("adresse");

                entity.Property(e => e.Nom)
                    .HasMaxLength(255)
                    .HasColumnName("nom");
            });

            modelBuilder.Entity<Grossiste>(entity =>
            {
                entity.ToTable("Grossiste");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Nom)
                    .HasMaxLength(255)
                    .HasColumnName("nom");

                entity.Property(e => e.Prix).HasColumnName("prix");

                entity.Property(e => e.Stock).HasColumnName("stock");
            });

            modelBuilder.Entity<Vente>(entity =>
            {
                entity.ToTable("Vente");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BiereId).HasColumnName("biere_id");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.GrossisteId).HasColumnName("grossiste_id");

                entity.Property(e => e.MontantHtva).HasColumnName("montantHTVA");

                entity.Property(e => e.Qtevendue).HasColumnName("qtevendue");

                entity.HasOne(d => d.Biere)
                    .WithMany(p => p.Ventes)
                    .HasForeignKey(d => d.BiereId)
                    .HasConstraintName("FK__Vente__biere_id__3E52440B");

                entity.HasOne(d => d.Grossiste)
                    .WithMany(p => p.Ventes)
                    .HasForeignKey(d => d.GrossisteId)
                    .HasConstraintName("FK__Vente__grossiste__3F466844");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
