using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using MVC.Data;

namespace MVC.Controllers; 

[Authorize(Roles = "RegionalManager, CentralManager")]
public class StudentsController : Controller {
    private readonly DataContext _context;

    public StudentsController(DataContext context) {
        _context = context;
    }

    // GET: Students
    public async Task<IActionResult> Index() {
        var dataContext = _context.Students.Include(s => s.City).Include(s => s.Group).Include(s => s.User);
        return View(await dataContext.ToListAsync());
    }

    // GET: Students/Details/5
    public async Task<IActionResult> Details(int? id) {
        if (id == null || _context.Students == null) {
            return NotFound();
        }
        if (!_context.IsUserAccessToStudentGranted(User, (int)id)) {
            return Problem("You not permitted to this student.");
        }

        var student = await _context.Students
            .Include(s => s.City)
            .Include(s => s.Group)
            .Include(s => s.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (student == null) {
            return NotFound();
        }

        return View(student);
    }

    // GET: Students/Create
    public IActionResult Create() {
        ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id");
        ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id");
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Login");
        return View();
    }

    // POST: Students/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,FullName,UserId,GroupId,CityId")] Student student) {
        if (!_context.IsUserAccessToStudentGranted(User, student.Id)) {
            return Problem("You not permitted to this student.");
        }
        if (ModelState.IsValid) {
            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", student.CityId);
        ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", student.GroupId);
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Login", student.UserId);
        return View(student);
    }

    // GET: Students/Edit/5
    public async Task<IActionResult> Edit(int? id) {
        if (id == null || _context.Students == null) {
            return NotFound();
        }
        
        if (!_context.IsUserAccessToStudentGranted(User, (int)id)) {
            return Problem("You not permitted to this student.");
        }

        var student = await _context.Students.FindAsync(id);
        if (student == null) {
            return NotFound();
        }

        ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", student.CityId);
        ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", student.GroupId);
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Login", student.UserId);
        return View(student);
    }

    // POST: Students/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,UserId,GroupId,CityId")] Student student) {
        if (id != student.Id) {
            return NotFound();
        }

        if (!_context.IsUserAccessToStudentGranted(User, id)) {
            return Problem("You not permitted to this student.");
        }
        
        if (ModelState.IsValid) {
            try {
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!StudentExists(student.Id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", student.CityId);
        ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", student.GroupId);
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Login", student.UserId);
        return View(student);
    }

    // GET: Students/Delete/5
    public async Task<IActionResult> Delete(int? id) {
        if (id == null || _context.Students == null) {
            return NotFound();
        }

        if (!_context.IsUserAccessToStudentGranted(User, (int)id)) {
            return Problem("You not permitted to this student.");
        }
        
        var student = await _context.Students
            .Include(s => s.City)
            .Include(s => s.Group)
            .Include(s => s.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (student == null) {
            return NotFound();
        }

        return View(student);
    }

    // POST: Students/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) {
        if (_context.Students == null) {
            return Problem("Entity set 'DataContext.Students'  is null.");
        }

        var student = await _context.Students.FindAsync(id);
        if (student != null) {
            if (!_context.IsUserAccessToStudentGranted(User, id)) {
                return Problem("You not permitted to this student.");
            }
            _context.Students.Remove(student);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool StudentExists(int id) {
        return (_context.Students?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}