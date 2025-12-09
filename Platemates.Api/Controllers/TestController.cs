using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platemates.Infrastructure.Persistence;
using Platemates.Domain.Entities;

namespace Platemates.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TestController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/test
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestEntity>>> GetAll()
    {
        return await _context.TestEntities.ToListAsync();
    }

    // POST: api/test
    [HttpPost]
    public async Task<ActionResult<TestEntity>> Create([FromBody] string name)
    {
        var entity = new TestEntity { Name = name };
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, entity);
    }
}