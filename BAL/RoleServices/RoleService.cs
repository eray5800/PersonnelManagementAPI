using DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace BAL.RoleServices
{
    public class RoleService 
    {
        private readonly UserManager<Employee> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public RoleService(UserManager<Employee> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task SeedRolesAsync()
        {
            string[] roleNames = { "SystemAdministrator","CompanyAdministrator", "Employee" };

            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                return await _userManager.GetRolesAsync(user); 
            }
            return Enumerable.Empty<string>();
        }


        public async Task AssignRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _roleManager.RoleExistsAsync(roleName))
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, roles);
                }
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }

        public async Task AssignRoleAsync(Employee user, string roleName)
        {
            if (user != null && await _roleManager.RoleExistsAsync(roleName))
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, roles);
                }
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }




        public async Task RemoveRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }
        }


        public async Task ChangeUserRoleAsync(string userId, string newRoleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _roleManager.RoleExistsAsync(newRoleName))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }
                await _userManager.AddToRoleAsync(user, newRoleName);
            }
        }


        public async Task<bool> CheckUserRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return await _userManager.IsInRoleAsync(user, role);
            }
            return false;
        }
    }
}
