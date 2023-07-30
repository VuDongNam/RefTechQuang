using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyTemplate.Data;

namespace MyTemplate.Areas.Identity.Pages.Role
{
  public class AddUserRoleModel : PageModel
  {
    private readonly ILogger<AddUserRoleModel> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public AddUserRoleModel(ILogger<AddUserRoleModel> logger, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
    {
      _logger = logger;
      _roleManager = roleManager;
      _userManager = userManager;
    }

    public class InputModel
    {
      [Required]
      public string Id { set; get; }
      public string Name { set; get; }

      public string[] RoleNames { set; get; }
    }

    [BindProperty]
    public InputModel Input { set; get; }

    [BindProperty]
    public bool isConfirmed { set; get; }

    [TempData]
    public string StatusMessage { get; set; }

    public IActionResult OnGet() => NotFound("Not Found");

    public List<string> AllRoles { set; get; } = new List<string>();

    public async Task<IActionResult> OnPost()
    {
      var user = await _userManager.FindByIdAsync(Input.Id);
      if (user == null)
      {
        return NotFound("Not Found");
      }

      var roles = await _userManager.GetRolesAsync(user);
      var allRoles = await _roleManager.Roles.ToListAsync();

      // copy role name vào AllRoles
      allRoles.ForEach((r) =>
      {
        AllRoles.Add(r.Name);
      });

      if (!isConfirmed)
      {
        Input.RoleNames = roles.ToArray();
        isConfirmed = true;
        StatusMessage = string.Empty;
        ModelState.Clear();
      }
      else
      {
        StatusMessage = "Updating";
        if (Input.RoleNames == null)
        {
          Input.RoleNames = new string[] { };
        }
        
        // add new role
        foreach (var roleName in Input.RoleNames)
        {
          if (roles.Contains(roleName))
            continue;
          await _userManager.AddToRoleAsync(user, roleName);
        }
        
        // clear old role
        foreach (var roleName in roles)
        {
          if (Input.RoleNames.Contains(roleName))
            continue;
          await _userManager.RemoveFromRoleAsync(user, roleName);
        }
      }

      Input.Name = user.UserName;
      return Page();
    }
  }
}
