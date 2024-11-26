using E = backend_c_.Entity;
using Microsoft.EntityFrameworkCore;

namespace backend_c_;
public class AppDbContext : DbContext
{
  public AppDbContext( DbContextOptions<AppDbContext> options )
      : base( options ) { }

  public DbSet<E.User> Users { get; set; }
  public DbSet<E.File> Files { get; set; }
  public DbSet<E.SharedFile> SharedFiles { get; set; }
  public DbSet<E.AccessLog> AccessLogs { get; set; }
  public DbSet<E.Version> Versions { get; set; }

  protected override void OnModelCreating( ModelBuilder modelBuilder )
  {
    modelBuilder.Entity<E.SharedFile>()
      .HasOne( sf => sf.File )
      .WithMany()
      .HasForeignKey( sf => sf.FileId )
      .OnDelete( DeleteBehavior.Cascade );

    modelBuilder.Entity<E.SharedFile>()
      .HasOne( sf => sf.Owner )
      .WithMany()
      .HasForeignKey( sf => sf.OwnerId )
      .OnDelete( DeleteBehavior.Restrict );

    modelBuilder.Entity<E.SharedFile>()
      .HasOne( sf => sf.SharedWith )
      .WithMany()
      .HasForeignKey( sf => sf.SharedWithId )
      .OnDelete( DeleteBehavior.Restrict );

    modelBuilder.Entity<E.File>()
      .HasOne( f => f.User )
      .WithMany()
      .HasForeignKey( f => f.UserId )
      .OnDelete( DeleteBehavior.Cascade );

    modelBuilder.Entity<E.AccessLog>()
      .HasOne( al => al.File )
      .WithMany()
      .HasForeignKey( al => al.FileId )
      .OnDelete( DeleteBehavior.Cascade );

    modelBuilder.Entity<E.AccessLog>()
      .HasOne( al => al.User )
      .WithMany()
      .HasForeignKey( al => al.UserId )
      .OnDelete( DeleteBehavior.Cascade );

    modelBuilder.Entity<E.Version>()
      .HasOne( v => v.File )
      .WithMany()
      .HasForeignKey( v => v.FileId )
      .OnDelete( DeleteBehavior.Cascade );
  }
}
