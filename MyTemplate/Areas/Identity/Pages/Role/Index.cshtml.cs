using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MyTemplate.Areas.Identity.Pages.Role
{
  [Authorize(Roles="Admin")]
  public class IndexModel : PageModel
  {
    private readonly ILogger<IndexModel> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;

    // sử dụng Attribute thay cho việc gán trực tiếp: TempData["StatusMessage"] = "Msg";
    [TempData]
    public string StatusMessage { set; get; }

    public List<IdentityRole> Roles { set; get; }

    public IndexModel(ILogger<IndexModel> logger, RoleManager<IdentityRole> roleManager)
    {
      _logger = logger;
      _roleManager = roleManager;
    }

    public async Task<IActionResult> OnGetAsync()
    {
      Roles = await _roleManager.Roles.ToListAsync();
      return Page();
    }
  }
}
