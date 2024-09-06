using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipManagement.Data;
using ShipManagement.Models.Roles;
using ShipManagement.Models.Users;

namespace ShipManagement.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ShipManagementDbContext _context;

        public UserManagementController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ShipManagementDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var userRoleInfos = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoleInfos.Add(new UserViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles
                });
            }

            return View(userRoleInfos);
        }
        
        [Authorize(Policy = "CanAssignTasks")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [Authorize(Policy = "CanAssignTasks")]
        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Username, Email = model.Email, PhoneNumber = model.PhoneNumber};
                
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var model = new EditUserViewModel() { Id = user.Id, Username = user.UserName, PhoneNumber = user.PhoneNumber};
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return NotFound("User not found.");
            }
            user.UserName = model.Username;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            
            if (result.Succeeded)
            {
                return RedirectToAction("Index"); 
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
        
        [Authorize(Policy = "CanAssignTasks")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            if (_context.Tasks.Any(t => t.AssignedToId == user.Id) || _context.Tasks.Any(t => t.AssignedById == user.Id))
            {
                return RedirectToAction("ConfirmDelete", new { id = user.Id });
            }
            
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error deleting user");
            }
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            
            if (user == null)
            {
                return NotFound();
            }

            var userTasks = _context.Tasks.Where(t => t.AssignedToId == user.Id).ToList();
            userTasks.AddRange(_context.Tasks.Where(t => t.AssignedById == user.Id).ToList());
            
            var viewModel = new ConfirmDeleteViewModel
            {
                User = user,
                Tasks = userTasks
            };

            return View(viewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> ConfirmDeleteConfirmed(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userTasks = _context.Tasks.Where(t => t.AssignedToId == user.Id).ToList();
            userTasks.AddRange(_context.Tasks.Where(t => t.AssignedById == user.Id).ToList());
            _context.Tasks.RemoveRange(userTasks);

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Error deleting user");
            return RedirectToAction("Index");
        }

        
        [Authorize(Policy = "CanAssignTasks")]
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            var model = new UserRolesViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Roles = userRoles,
                AllRoles = allRoles.Select(role => new RoleViewModel
                {
                    RoleName = role.Name,
                    Selected = userRoles.Contains(role.Name)
                }).ToList()
            };

            return View(model);
        }
        
        [Authorize(Policy = "CanAssignTasks")]
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(UserRolesViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Id))
            {
                return BadRequest("Invalid model or user ID.");
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove roles.");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.SelectedRole))
            {
                var addResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to add role.");
                    return View(model);
                }
            }

            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user); // Pass the user to the view
        }
    }
}
