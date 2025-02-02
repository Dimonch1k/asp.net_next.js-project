using System;
using backend_c_;
using Microsoft.AspNetCore.Mvc;
using VirusTotalMicroService.Entity;
using VirusTotalMicroService.Service;

namespace VirusTotalMicroService.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class ScanController : ControllerBase
{
  private readonly AppDbContext _dbContext;
  private readonly VirusTotalScannerService _totalScannerService;

  public ScanController( AppDbContext dbContext, VirusTotalScannerService virusTotalScannerService )
  {
    _dbContext = dbContext;
    _totalScannerService = virusTotalScannerService;
  }

  [HttpPost( "request-scan" )]
  public async Task<IActionResult> RequestScan( [FromForm] IFormFile file )
  {
    if ( file == null || file.Length == 0 )
    {
      return BadRequest( "No file uploaded." );
    }

    string response = await _totalScannerService.ScanFileWithVirusTotal( file );

    if ( response == "Infected" )
    {
      return BadRequest( "The file has viruses." );
    }

    return Ok( response );
  }

  [HttpGet( "scan-status/{id}" )]
  public IActionResult GetScanStatus( Guid id )
  {
    FileScanRequest? scanRequest = _dbContext.FileScanRequests.Find( id );

    if ( scanRequest == null )
    {
      return NotFound( "Scan request not found." );
    }

    return Ok( scanRequest );
  }

  [HttpGet]
  public List<FileScanRequest> GetFileScanRequests( ) => _dbContext.FileScanRequests.ToList();
}
