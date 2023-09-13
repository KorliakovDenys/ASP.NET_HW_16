using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using MVC.Data;

namespace MVC.Controllers;

[Authorize]
public class GradesController : Controller {
    private readonly DataContext _context;

    public GradesController(DataContext context) {
        _context = context;
    }

    // GET: Grades
    public async Task<IActionResult> Index() {
        var dataContext = _context.Grades!.Include(g => g.Course)
            .Include(g => g.Student);
        return View(await dataContext.ToListAsync());
    }

    [Authorize(Roles = "RegionalManager, CentralManager")]
    // GET: Grades/Details/5
    public async Task<IActionResult> Details(int? id) {
        if (id == null || _context.Grades == null) {
            return NotFound();
        }

        var grade = await _context.Grades
            .Include(g => g.Course)
            .Include(g => g.Student)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (grade == null) {
            return NotFound();
        }

        return View(grade);
    }

    [Authorize(Roles = "RegionalManager, CentralManager")]
    // GET: Grades/Create
    public IActionResult Create() {
        ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id");
        ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id");
        return View();
    }

    // POST: Grades/Create
    [Authorize(Roles = "RegionalManager, CentralManager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Value,StudentId,CourseId")] Grade grade) {
        if (!_context.IsUserAccessToStudentGranted(User, grade.StudentId)) {
            return Problem("You not permitted to this student.");
        }
        if (ModelState.IsValid) {
            _context.Add(grade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", grade.CourseId);
        ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", grade.StudentId);
        return View(grade);
    }

    [Authorize(Roles = "RegionalManager, CentralManager")]
    // GET: Grades/Edit/5
    public async Task<IActionResult> Edit(int? id) {
        if (id == null || _context.Grades == null) {
            return NotFound();
        }

        var grade = await _context.Grades.FindAsync(id);
        if (grade == null) {
            return NotFound();
        }

        ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", grade.CourseId);
        ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", grade.StudentId);
        return View(grade);
    }

    // POST: Grades/Edit/5
    [Authorize(Roles = "RegionalManager, CentralManager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Value,StudentId,CourseId")] Grade grade) {
        if (!_context.IsUserAccessToStudentGranted(User, grade.StudentId)) {
            return Problem("You not permitted to this student.");
        }

        if (id != grade.Id) {
            return NotFound();
        }

        if (ModelState.IsValid) {
            try {
                _context.Update(grade);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!GradeExists(grade.Id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", grade.CourseId);
        ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", grade.StudentId);
        return View(grade);
    }

    // GET: Grades/Delete/5
    [Authorize(Roles = "RegionalManager, CentralManager")]
    public async Task<IActionResult> Delete(int? id) {
        if (id == null || _context.Grades == null) {
            return NotFound();
        }

        var grade = await _context.Grades
            .Include(g => g.Course)
            .Include(g => g.Student)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (grade == null) {
            return NotFound();
        }
        if (!_context.IsUserAccessToStudentGranted(User, grade.StudentId)) {
            return Problem("You not permitted to this grade.");
        }
        return View(grade);
    }

    // POST: Grades/Delete/5
    [Authorize(Roles = "RegionalManager, CentralManager")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) {
        if (_context.Grades == null) {
            return Problem("Entity set 'DataContext.Grades'  is null.");
        }

        var grade = await _context.Grades!.FindAsync(id);
        if (grade != null) {
            if (!_context.IsUserAccessToStudentGranted(User, grade.StudentId)) {
                return Problem("You not permitted to this grade.");
            }

            _context.Grades.Remove(grade);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    private bool GradeExists(int id) {
        return (_context.Grades?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}