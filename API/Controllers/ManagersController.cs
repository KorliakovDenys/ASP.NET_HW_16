using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "CentralManager")]
[ApiController]
public class ManagersController : ControllerBase {
    private readonly DataContext _context;

    public ManagersController(DataContext context) {
        _context = context;
    }

    // GET: api/Managers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Manager>>> GetManagers() {
        if (_context.Managers == null) {
            return NotFound();
        }

        return await _context.Managers.ToListAsync();
    }

    // GET: api/Managers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Manager>> GetManager(int id) {
        if (_context.Managers == null) {
            return NotFound();
        }

        var manager = await _context.Managers.FindAsync(id);

        if (manager == null) {
            return NotFound();
        }

        return manager;
    }

    // PUT: api/Managers/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutManager(int id, Manager manager) {
        if (id != manager.Id) {
            return BadRequest();
        }

        _context.Entry(manager).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) {
            if (!ManagerExists(id)) {
                return NotFound();
            }
            else {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Managers
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Manager>> PostManager(Manager manager) {
        if (_context.Managers == null) {
            return Problem("Entity set 'DataContext.Managers'  is null.");
        }

        _context.Managers.Add(manager);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetManager", new { id = manager.Id }, manager);
    }

    // DELETE: api/Managers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteManager(int id) {
        if (_context.Managers == null) {
            return NotFound();
        }

        var manager = await _context.Managers.FindAsync(id);
        if (manager == null) {
            return NotFound();
        }

        _context.Managers.Remove(manager);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ManagerExists(int id) {
        return (_context.Managers?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}