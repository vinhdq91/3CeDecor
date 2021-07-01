using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleAdminController : BaseController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleAdminController(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public ActionResult<List<ApplicationRole>> RoleList()
        {
            var roles = _roleManager.Roles;
            List<ApplicationRole> listRoles = new List<ApplicationRole>();
            if (roles != null)
            {
                foreach (ApplicationRole roleItem in roles)
                {
                    listRoles.Add(roleItem);
                }
            }
            return View(listRoles);
        }

        #region Create role
        [HttpGet]
        public IActionResult CreateRole()
        {
            string errorMessage = (string)TempData["CreateRoleErrorMessage"];
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                // We just need to specify a unique role name to create a new role
                ApplicationRole identityRole = new ApplicationRole
                {
                    Name = model.RoleName,
                    CreatedDate = DateTime.Now,
                    Description = model.Description
                };

                // Saves the role in the underlying AspNetRoles table
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    TempData["CreateRoleErrorMessage"] = result.Errors.ToList()[0].Code;
                    return RedirectToAction("CreateRole");
                }
            }

            return RedirectToAction("RoleList");
        }
        #endregion

        #region Edit role
        [HttpGet]
        public async Task<ActionResult<ApplicationRole>> EditRole(string roleId)
        {
            // Get Role Info
            ApplicationRole roleEntity = await _roleManager.FindByIdAsync(roleId);

            // Get user belong to this role
            List<ApplicationUser> listUsers = new List<ApplicationUser>();
            foreach (ApplicationUser user in _userManager.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, roleEntity.Name))
                {
                    listUsers.Add(user);
                }
            }
            ViewBag.ListUsers = listUsers;
            return View(roleEntity);
        }

        [HttpPost]
        public async Task<ActionResult<ApplicationRole>> EditRole(ApplicationRole roleInput)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole roleEntity = await _roleManager.FindByIdAsync(roleInput.Id);
                if (roleEntity != null)
                {
                    roleEntity.Name = roleInput.Name;
                    roleEntity.Description = roleInput.Description;
                    var result = await _roleManager.UpdateAsync(roleEntity);
                    if (result.Succeeded)
                        return RedirectToAction("RoleList");
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            return RedirectToAction("RoleList");
        }
        #endregion

        #region Delete role
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            if (!string.IsNullOrEmpty(roleId))
            {
                ApplicationRole roleEntity = await _roleManager.FindByIdAsync(roleId);
                if (roleEntity != null)
                {
                    IdentityResult result = await _roleManager.DeleteAsync(roleEntity);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("RoleList");
                    }
                }
            }
            return RedirectToAction("RoleList");
        }
        #endregion

        #region Users in Role
        [HttpGet]
        public async Task<IActionResult> AddUsersToRole(string roleId)
        {
            ApplicationRole roleEntity = await _roleManager.FindByIdAsync(roleId);

            if (roleEntity == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            List<UserRoleModel> listUserRoleModel = new List<UserRoleModel>();
            foreach (ApplicationUser userItem in _userManager.Users.ToList())
            {
                var userRoleModel = new UserRoleModel
                {
                    UserId = userItem.Id,
                    UserName = userItem.UserName
                };

                if (!await _userManager.IsInRoleAsync(userItem, roleEntity.Name))
                {
                    userRoleModel.IsSelected = false;
                    listUserRoleModel.Add(userRoleModel);
                }
            }

            ViewBag.RoleId = roleId;
            return View(listUserRoleModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddUsersToRole(List<UserRoleModel> userRoleModel, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < userRoleModel.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(userRoleModel[i].UserId);

                IdentityResult result = null;

                if (userRoleModel[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (userRoleModel.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { roleId = roleId });
                }
            }

            return RedirectToAction("EditRole", new { roleId = roleId });
        }

        public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            ApplicationUser userEntity = await _userManager.FindByIdAsync(userId);
            ApplicationRole roleEntity = await _roleManager.FindByNameAsync(roleName);
            if (userEntity != null)
            {
                IdentityResult result = await _userManager.RemoveFromRoleAsync(userEntity, roleName);
                if (result.Succeeded)
                {
                    return RedirectToAction("EditRole", new { roleId = roleEntity.Id });
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return RedirectToAction("EditRole", new { roleId = roleEntity.Id });
        }
        #endregion
    }
}