using backend_c_;
using RestSharp;
using VirusTotalMicroService.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace VirusTotalMicroService.Service;

public class VirusTotalScannerService : BackgroundService
{
  private readonly string? _virusTotalApiKey = Environment.GetEnvironmentVariable( "VirusTotalApiKey" );
  private readonly ILogger<VirusTotalScannerService> _logger;
  private readonly IServiceProvider _serviceProvider;

  public VirusTotalScannerService( ILogger<VirusTotalScannerService> logger, IServiceProvider serviceProvider )
  {
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  protected override async Task ExecuteAsync( CancellationToken stoppingToken )
  {
    while ( !stoppingToken.IsCancellationRequested )
    {
      using ( IServiceScope scope = _serviceProvider.CreateScope() )
      {
        AppDbContext _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        List<FileScanRequest> pendingScanRequests = _dbContext.FileScanRequests
          .Where( r => r.Status == "Pending" )
          .ToList();

        foreach ( FileScanRequest scanRequest in pendingScanRequests )
        {
          scanRequest.Status = "Scanning";
          _dbContext.SaveChanges();

          string scanResult = await ScanFileWithVirusTotal( scanRequest.FilePath );

          if ( scanResult == "Clean" )
          {
            scanRequest.Status = "Clean";
            scanRequest.ScanResult = "No threats found.";
          }
          else
          {
            scanRequest.Status = "Infected";
            scanRequest.ScanResult = scanResult;
          }

          _dbContext.SaveChanges();
        }
      }

      await Task.Delay( 1000, stoppingToken );
    }
  }

  public async Task<string> ScanFileWithVirusTotal( IFormFile file )
  {
    RestClientOptions options = new RestClientOptions( "https://www.virustotal.com/api/v3/files" );
    RestClient client = new RestClient( options );
    RestRequest request = new()
    {
      AlwaysMultipartFormData = true
    };
    request.AddHeader( "accept", "application/json" );
    request.AddHeader( "x-apikey", _virusTotalApiKey );

    byte[] fileBytes = new byte[file.Length];
    using ( Stream stream = file.OpenReadStream() )
    {
      int bytesRead = await stream.ReadAsync( fileBytes, 0, fileBytes.Length );
    }

    request.AddFile( "file", fileBytes, file.FileName );

    RestResponse response = await client.PostAsync( request );

    if ( response.IsSuccessful )
    {
      return "Clean";
    }

    return "Infected";
  }

  public async Task<string> ScanFileWithVirusTotal( string filePath )
  {
    RestClientOptions options = new RestClientOptions( "https://www.virustotal.com/api/v3/files" );
    RestClient client = new RestClient( options );
    RestRequest request = new()
    {
      AlwaysMultipartFormData = true
    };
    request.AddHeader( "accept", "application/json" );
    request.AddHeader( "x-apikey", _virusTotalApiKey );
    request.AddFile( "file", filePath );

    RestResponse response = await client.PostAsync( request );

    if ( response.IsSuccessful )
    {
      return "Clean";
    }

    return "Infected";
  }
}
