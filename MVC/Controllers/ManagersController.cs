using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using MVC.Data;

namespace MVC.Controllers; 

[Authorize(Roles = "CentralManager")]
public class ManagersController : Controller {
    private readonly DataContext _context;

    public ManagersController(DataContext context) {
        _context = context;
    }

    // GET: Managers
    public async Task<IActionResult> Index() {
        var dataContext = _context.Managers.Include(m => m.City).Include(m => m.User);
        return View(await dataContext.ToListAsync());
    }

    // GET: Managers/Details/5
    public async Task<IActionResult> Details(int? id) {
        if (id == null || _context.Managers == null) {
            return NotFound();
        }

        var manager = await _context.Managers
            .Include(m => m.City)
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (manager == null) {
            return NotFound();
        }

        return View(manager);
    }

    // GET: Managers/Create
    public IActionResult Create() {
        ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id");
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Login");
        return View();
    }

    // POST: Managers/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,FullName,UserId,CityId")] Manager manager) {
        if (ModelState.IsValid) {
            _context.Add(manager);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", manager.CityId);
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Login", manager.UserId);
        return View(manager);
    }

    // GET: Managers/Edit/5
    public async Task<IActionResult> Edit(int? id) {
        if (id == null || _context.Managers == null) {
            return NotFound();
        }

        var manager = await _context.Managers.FindAsync(id);
        if (manager == null) {
            return NotFound();
        }

        ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", manager.CityId);
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Login", manager.UserId);
        return View(manager);
    }

    // POST: Managers/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,UserId,CityId")] Manager manager) {
        if (id != manager.Id) {
            return NotFound();
        }

        if (ModelState.IsValid) {
            try {
                _context.Update(manager);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ManagerExists(manager.Id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", manager.CityId);
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Login", manager.UserId);
        return View(manager);
    }

    // GET: Managers/Delete/5
    public async Task<IActionResult> Delete(int? id) {
        if (id == null || _context.Managers == null) {
            return NotFound();
        }

        var manager = await _context.Managers
            .Include(m => m.City)
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (manager == null) {
            return NotFound();
        }

        return View(manager);
    }

    // POST: Managers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) {
        if (_context.Managers == null) {
            return Problem("Entity set 'DataContext.Managers'  is null.");
        }

        var manager = await _context.Managers.FindAsync(id);
        if (manager != null) {
            _context.Managers.Remove(manager);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ManagerExists(int id) {
        return (_context.Managers?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}