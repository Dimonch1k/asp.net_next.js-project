using backend_c_.Entity;
using Microsoft.EntityFrameworkCore;

namespace backend_c_;
public class AppDbContext : DbContext
{
  public AppDbContext( DbContextOptions<AppDbContext> options )
      : base( options ) { }

  public DbSet<User> Users { get; set; }
  public DbSet<MediaFile> Files { get; set; }
  public DbSet<SharedFile> SharedFiles { get; set; }
  public DbSet<AccessLog> AccessLogs { get; set; }
  public DbSet<FileVersion> FileVersions { get; set; }
  public DbSet<Notification> Notifications { get; set; }
  public DbSet<FileScanRequest> FileScanRequests { get; set; }

  protected override void OnModelCreating( ModelBuilder modelBuilder )
  {
    modelBuilder.Entity<MediaFile>()
      .HasOne( f => f.User )
      .WithMany()
      .HasForeignKey( f => f.UserId )
      .OnDelete( DeleteBehavior.Cascade );

    modelBuilder.Entity<SharedFile>()
      .HasOne( sf => sf.File )
      .WithMany()
      .HasForeignKey( sf => sf.FileId )
      .OnDelete( DeleteBehavior.Cascade );

    modelBuilder.Entity<SharedFile>()
      .HasOne( sf => sf.Owner )
      .WithMany()
      .HasForeignKey( sf => sf.OwnerId )
      .OnDelete( DeleteBehavior.Cascade );

    modelBuilder.Entity<SharedFile>()
      .HasOne( sf => sf.SharedWith )
      .WithMany()
      .HasForeignKey( sf => sf.SharedWithId )
      .OnDelete( DeleteBehavior.SetNull );

    modelBuilder.Entity<AccessLog>()
      .HasOne( al => al.SharedFile )
      .WithMany()
      .HasForeignKey( al => al.SharedFileId )
      .OnDelete( DeleteBehavior.Cascade );

    modelBuilder.Entity<AccessLog>()
      .HasOne( al => al.User )
      .WithMany()
      .HasForeignKey( al => al.UserId )
      .OnDelete( DeleteBehavior.Cascade );

    modelBuilder.Entity<FileVersion>()
      .HasOne( v => v.File )
      .WithMany()
      .HasForeignKey( v => v.FileId )
      .OnDelete( DeleteBehavior.Cascade );

    modelBuilder.Entity<Notification>()
        .HasOne( n => n.User )
        .WithMany()
        .HasForeignKey( n => n.UserId )
        .OnDelete( DeleteBehavior.Cascade );
  }
}
