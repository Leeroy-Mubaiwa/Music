using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Music.Models;

namespace Music.Data;

public partial class MusicDbContext : DbContext
{
    public MusicDbContext()
    {
    }

    public MusicDbContext(DbContextOptions<MusicDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<Detection> Detections { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleClaim> RoleClaims { get; set; }

    public virtual DbSet<Track> Tracks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserClaim> UserClaims { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=musicConn");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__albums__3213E83F585731EE");

            entity.Property(e => e.ReleaseDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Artist).WithMany(p => p.Albums).HasConstraintName("FK__albums__artist_i__5EBF139D");
        });

        modelBuilder.Entity<Detection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__detectio__3213E83F1E29A712");

            entity.Property(e => e.DateDetected).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Track).WithMany(p => p.Detections).HasConstraintName("FK__detection__track__787EE5A0");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__messages__3213E83F56ECECBA");

            entity.Property(e => e.DateSent).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.SourceUserNavigation).WithMany(p => p.MessageSourceUserNavigations).HasConstraintName("FK__messages__source__70DDC3D8");

            entity.HasOne(d => d.TargetUserNavigation).WithMany(p => p.MessageTargetUserNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__messages__target__71D1E811");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");
        });

        modelBuilder.Entity<Track>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3213E83FEDD736EA");

            entity.HasOne(d => d.Album).WithMany(p => p.Tracks).HasConstraintName("fk_track_album");

            entity.HasOne(d => d.Artist).WithMany(p => p.Tracks).HasConstraintName("FK__tracks__artist_i__778AC167");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("UserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_UserRoles_RoleId");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
