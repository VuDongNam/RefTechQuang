using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MyTemplate.Areas.Identity.Pages.Role
{
    public class AddModel : PageModel
    {
        private readonly ILogger<AddModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        [TempData]
        public string StatusMessage { set; get; }

        public class CreateModel
        {
            public string Id { set; get; }
            [Required]
            public string Name { set; get; }
        }

        [BindProperty]
        public bool IsUpdatePage { set; get; } = false;

        [BindProperty]
        public CreateModel Input { set; get; }

        public AddModel(ILogger<AddModel> logger, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _roleManager = roleManager;
        }

        public IActionResult OnGet() => NotFound("Not Found");

        public IActionResult OnPost() => NotFound("Not Found");

        // hiển thị page Add
        public IActionResult OnPostStartNewRole()
        {
            StatusMessage = "Input Role values";
            IsUpdatePage = false;
            ModelState.Clear();
            return Page();
        }

        public async Task<IActionResult> OnPostStartUpdate()
        {
            StatusMessage = string.Empty;
            var role = await _roleManager.FindByIdAsync(Input.Id);
            if (role != null) 
            {
                Input.Name = role.Name;
                IsUpdatePage = true;
                ModelState.Clear();
            }
            else
            {
                StatusMessage = $"Error: Role Id = {Input.Id} Not Found.";
            }
            return Page();
        }


        public async Task<IActionResult> OnPostSave()
        {
            if (IsUpdatePage)
            {
                // trường hợp udpate
                if (Input.Id == null)
                {
                    ModelState.Clear();
                    StatusMessage = "Error: Role not Found.";
                    return Page();
                }
                else
                {
                    var role = await _roleManager.FindByIdAsync(Input.Id);
                    if (role != null)
                    {
                        role.Name = Input.Name;
                        // save role to db;
                        var savedRole = await _roleManager.UpdateAsync(role);
                        if (savedRole.Succeeded)
                        {
                            StatusMessage = "Update role Successully.";
                        }
                        else
                        {
                            StatusMessage = "Error: ";
                            foreach (var item in savedRole.Errors)
                            {
                                StatusMessage += item.Description;
                            }
                        }
                    }
                    else
                    {
                        StatusMessage = $"Error: Role Id = {Input.Id} Not Found.";
                    }
                }
            }
            else
            {   
                // trường hợp create new role
                var role = new IdentityRole(Input.Name);
                var createdRole = await _roleManager.CreateAsync(role);
                if (createdRole.Succeeded)
                {
                    StatusMessage = "Created New Role Successfully.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    StatusMessage = "Error: ";
                    foreach (var item in createdRole.Errors)
                    {
                        StatusMessage += item.Description;
                    }
                }
            }

            return Page();
        }
    }
}
