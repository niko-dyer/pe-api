using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers
{
    public class UserResponse 
    {
        public string? Id { get; set; }
        public int? DisplayId { get; set; }
        public string? FullName { get; set; }
        public string? Title { get; set; }
        public string? Role { get; set; }
        public string? Department { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly webapiContext _context;
        private readonly ApplicationUserManager _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(webapiContext context, ApplicationUserManager userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetCurrentUser")]
        [Authorize(Roles = "Admin,Reviewer,Employee,Volunteer")]
        public async Task<ActionResult<UserResponse>> GetCurrentUser()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (_context.User == null)
            {
                return NotFound("The User table is currently empty.");
            }

            if (userId != null)
            {
                var result = (from a in _userManager.Users
                              join b in _context.UserRoles on a.Id equals b.UserId
                              join c in _roleManager.Roles on b.RoleId equals c.Id
                              where a.Id == userId
                              select new UserResponse
                              {
                                  Id = a.Id,
                                  DisplayId = a.DisplayId,
                                  FullName = a.FullName,
                                  Role = c.Name,
                                  Title = a.Title,
                                  Department = a.Department,
                                  PhoneNumber = a.PhoneNumber,
                                  Email = a.Email
                              }).SingleOrDefault();
                return result;
            }

            return NotFound("No user found");
        }

        // GET: api/Users
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Reviewer")]
        public async Task<ActionResult<IEnumerable>> GetUsers()
        {
          if (_context.User == null)
          {
              return NotFound("The user table is currently empty.");
          }

            var result = (from a in _userManager.Users
                          join b in _context.UserRoles on a.Id equals b.UserId into userRoles
                          from ur in userRoles.DefaultIfEmpty()
                          join c in _roleManager.Roles on ur.RoleId equals c.Id into roles
                          from r in roles.DefaultIfEmpty()
                          select new UserResponse
                          {
                              Id = a.Id,
                              DisplayId = a.DisplayId,
                              FullName = a.FullName,
                              Role = r.Name,
                              Title = a.Title,
                              Department = a.Department,
                              PhoneNumber = a.PhoneNumber,
                              Email = a.Email
                          }).ToList();
            return result;
        }

        // GET: api/Users/5
        [HttpGet("GetByHash/{id}")]
        [Authorize(Roles = "Admin,Reviewer")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            if (_context.User == null)
            {
                return NotFound("The User table is currently empty.");
            }
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("A user by that Id was not found.");
            }

            return user;
        }

        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "Admin,Reviewer")]
        public async Task<ActionResult<User>> GetUserByDisplayId(int id)
        {
            if (_context.User == null)
            {
                return NotFound("The User table is currently empty.");
            }
            var user = await _userManager.FindByDisplayIdAsync(id);

            if (user == null)
            {
                return NotFound("A user by that Id was not found.");
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Admin,Reviewer")]
        public async Task<IActionResult> PutUser(string id, UserResponse user)
        {
            if (id != user.Id)
            {
                return BadRequest("The Id provided and the new user info do not match.");
            }

            try
            {
                
                var userMatch = await _userManager.FindByIdAsync(id);
                if (userMatch is not null)
                {
                    userMatch.FullName = user.FullName;
                    userMatch.Title = user.Title;
                    userMatch.Department = user.Department;
                    userMatch.PhoneNumber = user.PhoneNumber;
                    userMatch.Email = user.Email;

                    var currentRoles = await _userManager.GetRolesAsync(userMatch);
                    if (currentRoles != null)
                    {
                        await _userManager.RemoveFromRolesAsync(userMatch, currentRoles);
                    }
                    await _userManager.AddToRoleAsync(userMatch, user.Role);
                    var result = await _userManager.UpdateAsync(userMatch);
                    if (result.Succeeded)
                    {
                        return Ok("User updated successfully!");
                    }
                    return BadRequest(result);
                }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound("DbUpdateConcurrency Error during user update.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("UpdateByEmail/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserByEmail(string email, UserResponse user)
        {
            if (email != user.Email)
            {
                return BadRequest("The email provided and the new user info do not match.");
            }

            try
            {

                var userMatch = await _userManager.FindByEmailAsync(email);
                if (userMatch is not null)
                {
                    userMatch.FullName = user.FullName;
                    userMatch.Title = user.Title;
                    userMatch.Department = user.Department;
                    userMatch.PhoneNumber = user.PhoneNumber;
                    userMatch.Email = user.Email;

                    var currentRoles = await _userManager.GetRolesAsync(userMatch);
                    if (currentRoles != null)
                    {
                        await _userManager.RemoveFromRolesAsync(userMatch, currentRoles);
                    }
                    await _userManager.AddToRoleAsync(userMatch, user.Role);
                    var result = await _userManager.UpdateAsync(userMatch);
                    if (result.Succeeded)
                    {
                        return Ok("User updated successfully!");
                    }
                    return BadRequest(result);
                }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(email))
                {
                    return NotFound("DbUpdateConcurrency Error during user update.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (_context.User == null)
            {
                return NotFound("The user table is currently empty.");
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("There was no user found with Id" + id);
            }

            var result = await _userManager.DeleteAsync(user);
            
            if (result.Succeeded)
            {
                return Ok("User deleted successfully!");
            }
            return BadRequest("Something went wrong while deleting user " + id);
        }

        [HttpDelete("DeleteMultiple")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUsers(string[] idArray)
        {
            if (_context.User == null)
            {
                return NotFound("The user table is currently empty.");
            }
            foreach (string id in idArray)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound("There was no user found with Id" + id);
                }
                var result = await _userManager.DeleteAsync(user);
            }
            return Ok("All users with those Id's were successfully deleted.");
        }

        private bool UserExists(string id)
        {
            return (_context.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private bool UserExistsByEmail(string email)
        {
            return (_context.User?.Any(e => e.Email == email)).GetValueOrDefault();
        }
    }
}
