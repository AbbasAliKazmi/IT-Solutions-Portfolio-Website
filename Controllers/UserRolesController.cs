// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using ProductAPI.Models;
// using ProductAPI.Services;


// namespace ProductAPI.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//   //  [Authorize(Roles = "Admin")] //  Sirf Admin access
//     public class UserRolesController : ControllerBase
//     {
//         private readonly UserManager<ApplicationUser> _userManager;

//         public UserRolesController(UserManager<ApplicationUser> userManager)
//         {
//             _userManager = userManager;
//         }

//         //  1) Get all users + roles
//         [HttpGet]
//         public async Task<IActionResult> GetAllUsers()
//         {
//             var users = _userManager.Users.ToList();
//             var userList = new List<object>();

//             foreach (var user in users)
//             {
//                 var roles = await _userManager.GetRolesAsync(user);
//                 userList.Add(new
//                 {
//                     user.Id,
//                     user.UserName,
//                     user.Email,
//                     Roles = roles
//                 });
//             }

//             return Ok(userList);
//         }

//         //  2) Assign role to a user
//         [HttpPut("{userId}/assign")]
//         public async Task<IActionResult> AssignRole(string userId, [FromBody] string role)
//         {
//             var user = await _userManager.FindByIdAsync(userId);
//             if (user == null) return NotFound("User not found");

//             if (await _userManager.IsInRoleAsync(user, role))
//                 return BadRequest("User already in this role");

//             await _userManager.AddToRoleAsync(user, role);
//             return Ok($"Role '{role}' assigned to {user.UserName}");
//         }

//         //  3) Block a user (lockout enable)
//         [HttpPut("{userId}/block")]
//         public async Task<IActionResult> BlockUser(string userId)
//         {
//             var user = await _userManager.FindByIdAsync(userId);
//             if (user == null) return NotFound("User not found");

//             user.LockoutEnabled = true;
//             user.LockoutEnd = DateTimeOffset.MaxValue; // practically block forever
//             await _userManager.UpdateAsync(user);

//             return Ok($"{user.UserName} has been blocked.");
//         }

//         //  4) Unblock a user
//         [HttpPut("{userId}/unblock")]
//         public async Task<IActionResult> UnblockUser(string userId)
//         {
//             var user = await _userManager.FindByIdAsync(userId);
//             if (user == null) return NotFound("User not found");

//             user.LockoutEnd = null;
//             await _userManager.UpdateAsync(user);

//             return Ok($"{user.UserName} has been unblocked.");
//         }
//     }
// }
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using ProductAPI.Services;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRolesController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        //  1) Get all users + roles
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    Roles = roles
                });
            }

            return Ok(userList);
        }

        //  2) Assign role to a user  (UPDATED)
        [HttpPut("{userId}/assign")]
        public async Task<IActionResult> AssignRole(string userId, [FromBody] RoleAssignDTO dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var role = dto.Role;

            if (string.IsNullOrWhiteSpace(role))
                return BadRequest("Role is required");

            if (await _userManager.IsInRoleAsync(user, role))
                return BadRequest("User already in this role");

            await _userManager.AddToRoleAsync(user, role);

            return Ok($"Role '{role}' assigned to {user.UserName}");
        }

        //  3) Block a user
        [HttpPut("{userId}/block")]
        public async Task<IActionResult> BlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
            await _userManager.UpdateAsync(user);

            return Ok($"{user.UserName} has been blocked.");
        }

        //  4) Unblock a user
        [HttpPut("{userId}/unblock")]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);

            return Ok($"{user.UserName} has been unblocked.");
        }
    }
}
