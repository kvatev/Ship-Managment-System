using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;
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
        [Authorize(Policy = "CanAssignTasks")]
        public IActionResult Create()
        {
            ViewData["AssignedById"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["AssignedToId"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        // POST: Task/Create
        [Authorize(Policy = "CanAssignTasks")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,AssignedById,AssignedToId,AssignedDate,DueDate,Priority,IsCompleted")] TaskViewModel taskViewModel)
        {
            taskViewModel.Id = Guid.NewGuid();
            taskViewModel.AssignedById = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.Add(taskViewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
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
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,AssignedById,AssignedToId,AssignedDate,DueDate,Priority,IsCompleted")] TaskViewModel taskViewModel)
        {
            if (id != taskViewModel.Id)
            {
                return NotFound();
            }
            
            var existingTask = await _context.Tasks.FindAsync(id);

            if (existingTask == null)
            {
                return NotFound();
            }
            
            existingTask.Title = taskViewModel.Title;
            existingTask.Description = taskViewModel.Description;
            existingTask.AssignedById = taskViewModel.AssignedById;
            existingTask.AssignedToId = taskViewModel.AssignedToId;
            existingTask.AssignedDate = taskViewModel.AssignedDate;
            existingTask.DueDate = taskViewModel.DueDate;
            existingTask.Priority = taskViewModel.Priority;
            existingTask.IsCompleted = taskViewModel.IsCompleted;
            
            try
            {
                _context.Update(existingTask);
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

        // GET: Task/Delete/5
        [Authorize(Policy = "CanAssignTasks")]
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
        [Authorize(Policy = "CanAssignTasks")]
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
        
        // POST: Task/Update
        public async Task<IActionResult> MarkTaskAsCompleted(Guid taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
    
            if (task != null)
            {
                task.IsCompleted = true;
                task.CompletedDateTime = DateTime.Now;

                await _context.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }
        
        // GET: Task/Statistics
        public async Task<IActionResult> Statistics()
        {
            var tasks = new List<TaskViewModel>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (User.IsInRole("Администратор") || User.IsInRole("Адмирал") || User.IsInRole("Вицеадмирал") || User.IsInRole("Контраадмирал") || User.IsInRole("Флотилен адмирал"))
            {
                tasks = await _context.Tasks.ToListAsync();
            }
            else
            {
                tasks = await _context.Tasks.Where(t => t.AssignedToId == userId || t.AssignedById == userId).ToListAsync();
            }
            
            var totalTasks = tasks.Count;
            var completedTasks = tasks.Count(t => t.IsCompleted);
            var pendingTasks = totalTasks - completedTasks;
            
            var lowPriorityTasks = tasks.Count(t => t.Priority == TaskPriority.Нисък);
            var mediumPriorityTasks = tasks.Count(t => t.Priority == TaskPriority.Среден);
            var highPriorityTasks = tasks.Count(t => t.Priority == TaskPriority.Висок);

            // Calculate average completion time for completed tasks
            var averageCompletionTime = tasks.Where(t => t.CompletedDateTime.HasValue && t.AssignedDate != default(DateTime)).Select(t => (t.CompletedDateTime.Value - t.AssignedDate).TotalDays).DefaultIfEmpty(0).Average();

            var viewModel = new TaskStatisticsViewModel
            {
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                PendingTasks = pendingTasks,
                AverageCompletionTime = totalTasks > 0 ? (double?)averageCompletionTime : null,
                LowPriorityTasks = lowPriorityTasks,
                MediumPriorityTasks = mediumPriorityTasks,
                HighPriorityTasks = highPriorityTasks
            };

            return View(viewModel);
        }
    }
}
