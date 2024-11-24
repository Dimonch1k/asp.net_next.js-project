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
    base.OnModelCreating( modelBuilder );

    // Configure SharedFile -> File relationship
    modelBuilder.Entity<E.SharedFile>()
        .HasOne( sf => sf.File )
        .WithMany( f => f.SharedFiles )
        .HasForeignKey( sf => sf.FileId )
        .OnDelete( DeleteBehavior.Cascade );

    // Configure SharedFile -> Owner (User) relationship
    modelBuilder.Entity<E.SharedFile>()
        .HasOne( sf => sf.Owner )
        .WithMany( u => u.OwnedFiles )
        .HasForeignKey( sf => sf.OwnerId )
        .OnDelete( DeleteBehavior.Restrict );

    // Configure SharedFile -> SharedWith (User) relationship
    modelBuilder.Entity<E.SharedFile>()
        .HasOne( sf => sf.SharedWith )
        .WithMany( u => u.SharedWithFiles )
        .HasForeignKey( sf => sf.SharedWithId )
        .OnDelete( DeleteBehavior.Restrict );
  }
}
