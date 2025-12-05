using Microsoft.AspNetCore.Mvc;

namespace Platemates.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> GetHelloWorld()
    {
        var hello = "Hello World, Is this working? ?";

        return Ok(hello);
    }

    // [HttpGet]
    // [Route("ConnectionString")]
    // public ActionResult<string> GetConnectionString()
    // {
    //     var builder = WebApplication.CreateBuilder();

    //     var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    //     if (connectionString == null)
    //         return NotFound();

    //     return Ok(connectionString);
    // }
}