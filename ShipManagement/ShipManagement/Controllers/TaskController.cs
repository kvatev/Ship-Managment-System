using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipManagement.Data;
using ShipManagement.Models.Tasks;

namespace ShipManagement.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ShipManagementDbContext _context;

        public TaskController(ShipManagementDbContext context)
        {
            _context = context;
        }

        // GET: Task
        public async Task<IActionResult> Index()
        {
            var shipManagementDbContext = 
                _context.Tasks.Include(t => t.AssignedBy)
                    .Include(t => t.AssignedTo);
            
            return View(await shipManagementDbContext.ToListAsync());
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskViewModel = await _context.Tasks
                .Include(t => t.AssignedBy)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskViewModel == null)
            {
                return NotFound();
            }

            return View(taskViewModel);
        }

        // GET: Task/Create
        public IActionResult Create()
        {
            ViewData["AssignedById"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["AssignedToId"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,AssignedById,AssignedToId,AssignedDate,DueDate,IsCompleted")] TaskViewModel taskViewModel)
        {
            //if (ModelState.IsValid)
            {
                taskViewModel.Id = Guid.NewGuid();
                _context.Add(taskViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssignedById"] = new SelectList(_context.Users, "Id", "UserName", taskViewModel.AssignedById);
            ViewData["AssignedToId"] = new SelectList(_context.Users, "Id", "UserName", taskViewModel.AssignedToId);
            return View(taskViewModel);
        }
        [Authorize]
        // GET: Task/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskViewModel = await _context.Tasks.FindAsync(id);
            if (taskViewModel == null)
            {
                return NotFound();
            }
            ViewData["AssignedById"] = new SelectList(_context.Users, "Id", "UserName", taskViewModel.AssignedById);
            ViewData["AssignedToId"] = new SelectList(_context.Users, "Id", "UserName", taskViewModel.AssignedToId);
            return View(taskViewModel);
        }

        // POST: Task/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,AssignedById,AssignedToId,AssignedDate,DueDate,IsCompleted")] TaskViewModel taskViewModel)
        {
            if (id != taskViewModel.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskViewModelExists(taskViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssignedById"] = new SelectList(_context.Users, "Id", "UserName", taskViewModel.AssignedById);
            ViewData["AssignedToId"] = new SelectList(_context.Users, "Id", "UserName", taskViewModel.AssignedToId);
            return View(taskViewModel);
        }

        // GET: Task/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskViewModel = await _context.Tasks
                .Include(t => t.AssignedBy)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskViewModel == null)
            {
                return NotFound();
            }

            return View(taskViewModel);
        }

        // POST: Task/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var taskViewModel = await _context.Tasks.FindAsync(id);
            if (taskViewModel != null)
            {
                _context.Tasks.Remove(taskViewModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskViewModelExists(Guid id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
