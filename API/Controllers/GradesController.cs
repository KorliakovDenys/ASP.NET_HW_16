using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class GradesController : ControllerBase {
    private readonly DataContext _context;

    public GradesController(DataContext context) {
        _context = context;
    }

    // GET: api/Grades
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Grade>>> GetGrades() {
        if (_context.Grades == null) {
            return NotFound();
        }

        return await _context.Grades.ToListAsync();
    }

    // GET: api/Grades/5
    [HttpGet("{id}")]
    [Authorize(Roles = "RegionalManager, CentralManager")]
    public async Task<ActionResult<Grade>> GetGrade(int id) {
        if (_context.Grades == null) {
            return NotFound();
        }

        var grade = await _context.Grades.FindAsync(id);

        if (grade == null) {
            return NotFound();
        }

        return grade;
    }

    // PUT: api/Grades/5
    [HttpPut("{id}")]
    [Authorize(Roles = "RegionalManager, CentralManager")]
    public async Task<IActionResult> PutGrade(int id, Grade grade) {
        if (id != grade.Id) {
            return BadRequest();
        }
        if (!_context.IsUserAccessToStudentGranted(User, grade.StudentId)) {
            return Problem("You not permitted to this student.");
        }
        _context.Entry(grade).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) {
            if (!GradeExists(id)) {
                return NotFound();
            }
            else {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Grades
    [HttpPost]
    [Authorize(Roles = "RegionalManager, CentralManager")]
    public async Task<ActionResult<Grade>> PostGrade(Grade grade) {
        if (_context.Grades == null) {
            return Problem("Entity set 'DataContext.Grades'  is null.");
        }
        if (!_context.IsUserAccessToStudentGranted(User, grade.StudentId)) {
            return Problem("You not permitted to this student.");
        }
        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetGrade", new { id = grade.Id }, grade);
    }

    // DELETE: api/Grades/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "RegionalManager, CentralManager")]
    public async Task<IActionResult> DeleteGrade(int id) {
        if (_context.Grades == null) {
            return NotFound();
        }

        var grade = await _context.Grades.FindAsync(id);
        if (grade == null) {
            return NotFound();
        }
        if (!_context.IsUserAccessToStudentGranted(User, grade.StudentId)) {
            return Problem("You not permitted to this student.");
        }
        _context.Grades.Remove(grade);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool GradeExists(int id) {
        return (_context.Grades?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}