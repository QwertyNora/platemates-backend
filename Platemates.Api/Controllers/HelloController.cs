using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platemates.Infrastructure.Persistence;

namespace Platemates.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloController(ApplicationDbContext context, IConfiguration configuration) : ControllerBase
{

    private readonly ApplicationDbContext _context = context;
    private readonly IConfiguration _configuration = configuration;

    [HttpGet]
    public ActionResult<string> GetHelloWorld()
    {
        var hello = "Hello World";

        return Ok(hello);
    }

    [HttpGet("db-test")]
    public async Task<IActionResult> TestDatabase()
    {
        var diagnostics = new Dictionary<string, object>();

        var connString = _configuration.GetConnectionString("DefaultConnection");
        diagnostics["ActualConnectionString"] = connString;

        try
        {
            // var connString = _configuration.GetConnectionString("DefaultConnection");
            diagnostics["HasConnectionString"] = !string.IsNullOrEmpty(connString);
            diagnostics["ConnectionStringLength"] = connString?.Length ?? 0;

            // Försök öppna en faktisk connection för att få felmeddelande
            await using (var connection = _context.Database.GetDbConnection())
            {
                diagnostics["ConnectionState"] = connection.State.ToString();

                await connection.OpenAsync();

                diagnostics["ConnectionOpened"] = true;
                diagnostics["DatabaseName"] = connection.Database;
            }

            return Ok(diagnostics);
        }
        catch (Exception ex)
        {
            diagnostics["Exception"] = ex.Message;
            diagnostics["ExceptionType"] = ex.GetType().Name;
            diagnostics["InnerException"] = ex.InnerException?.Message ?? "null";
            diagnostics["FullException"] = ex.ToString();

            return StatusCode(500, diagnostics);
        }
    }
}