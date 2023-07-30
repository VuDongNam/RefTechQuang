using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyTemplate.Data;

namespace MyTemplate.Areas.Identity.Pages.Role
{
  public class UserModel : PageModel
  {
    const int USER_PER_PAGE = 10;

    private readonly ILogger<UserModel> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public class UserInList : AppUser
    {
      public string ListRoles { set; get; }
    }

    public List<UserInList> users;
    public int TotalPages { set; get; }

    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty(SupportsGet = true)]
    public int pageNumber { set; get; }

    public UserModel(ILogger<UserModel> logger, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
    {
      _logger = logger;
      _roleManager = roleManager;
      _userManager = userManager;
    }

    public void OnPost() => NotFound("Not Found");

    public async Task<IActionResult> OnGet()
    {
      var cUser = await _userManager.GetUserAsync(User);  // lấy user đang đăng nhập
      await _userManager.AddToRolesAsync(cUser, new string[] { "User" });

      // xử lý cho phần phân trang
      if (pageNumber == 0)
      {
        pageNumber = 1;
      }

      var listUsers = _userManager.Users
          .OrderBy(uu => uu.UserName)
          .Select(uu => new UserInList { Id = uu.Id, UserName = uu.UserName });

      // xử lý cho phần phân trang
      int totalUsers = await listUsers.CountAsync();

      // celling(2.2) = 3
      TotalPages = (int)Math.Ceiling((double)totalUsers / USER_PER_PAGE);

      users = await listUsers
          .Skip(USER_PER_PAGE * (pageNumber - 1))
          .Take(USER_PER_PAGE)
          .ToListAsync();

      foreach (var user in users)
      {
        var roles = await _userManager.GetRolesAsync(user);
        // nối các phần tử của list thành chuỗi, phân cách bằng dấu ','
        user.ListRoles = string.Join(", ", roles.ToList());
      }

      return Page();
    }
  }
}
