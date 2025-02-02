using Microsoft.EntityFrameworkCore;
using VirusTotalMicroService.Entity;

namespace backend_c_;
public class AppDbContext : DbContext
{
  public AppDbContext( DbContextOptions<AppDbContext> options ) : base( options ) { }

  public DbSet<FileScanRequest> FileScanRequests { get; set; }
}
