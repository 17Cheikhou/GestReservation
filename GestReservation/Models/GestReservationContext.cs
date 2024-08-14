using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GestReservation.Models;

public partial class GestReservationContext : DbContext
{
    public GestReservationContext()
    {
    }

    public GestReservationContext(DbContextOptions<GestReservationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Trajet> Trajets { get; set; }

    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }

    public virtual DbSet<Vehicule> Vehicules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("Reservation");

            entity.Property(e => e.DateReservation).HasColumnType("datetime");

            entity.HasOne(d => d.Trajet).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.TrajetId)
                .HasConstraintName("FK_Reservation_Trajet");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UtilisateurId)
                .HasConstraintName("FK_Reservation_Utilisateur");
        });

        modelBuilder.Entity<Trajet>(entity =>
        {
            entity.ToTable("Trajet");

            entity.Property(e => e.DateDepart).HasColumnType("datetime");
            entity.Property(e => e.VilleArrive)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VilleDepart)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Trajets)
                .HasForeignKey(d => d.UtilisateurId)
                .HasConstraintName("FK_Trajet_Utilisateur");
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.ToTable("Utilisateur");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Image)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MotDePass)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Prenom)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Roles)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Vehicule>(entity =>
        {
            entity.ToTable("Vehicule");

            entity.Property(e => e.Image)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Immatriculation)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Marque)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Modele)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Vehicules)
                .HasForeignKey(d => d.UtilisateurId)
                .HasConstraintName("FK_Vehicule_Utilisateur");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
