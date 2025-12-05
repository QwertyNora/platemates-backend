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
}