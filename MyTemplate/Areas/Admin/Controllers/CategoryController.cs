using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyTemplate.Data;

namespace MyTemplate.Areas.Admin.Controllers
{
  [Area("Admin")]
  public class CategoryController : Controller
  {
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
      _context = context;
    }

    // GET: Admin/Category
    public async Task<IActionResult> Index()
    {
      var cateWithChildren = _context.Categories
        .Include(c => c.ParentCategory)
        .Include(c => c.CategoryChildren);
      var categories = (await cateWithChildren.ToListAsync())
        .Where(c => c.ParentCategory == null)
        .ToList();

      //var appDbContext = _context.Categories.Include(c => c.ParentCategory);
      return View(categories);
    }

    // GET: Admin/Category/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var category = await _context.Categories
          .Include(c => c.ParentCategory)
          .FirstOrDefaultAsync(m => m.Id == id);
      if (category == null)
      {
        return NotFound();
      }

      return View(category);
    }

    async Task<IEnumerable<Category>> GetItemsSelectCategories()
    {
      var items = await _context.Categories
                          .Include(c => c.CategoryChildren)
                          .Where(c => c.ParentCategory == null)
                          .ToListAsync();

      List<Category> resultitems = new List<Category>() {
        new Category() {
            Id = -1,
            Title = "No Parent"
        }
      };
      Action<List<Category>, int> _ChangeTitleCategory = null;
      Action<List<Category>, int> ChangeTitleCategory = (items, level) => {
        string prefix = String.Concat(Enumerable.Repeat("—", level));
        foreach (var item in items)
        {
          item.Title = prefix + " " + item.Title;
          resultitems.Add(item);
          if ((item.CategoryChildren != null) && (item.CategoryChildren.Count > 0))
          {
            _ChangeTitleCategory(item.CategoryChildren.ToList(), level + 1);
          }

        }

      };

      _ChangeTitleCategory = ChangeTitleCategory;
      ChangeTitleCategory(items, 0);

      return resultitems;
    }

    // GET: Admin/Category/Create
    public async Task<IActionResult> Create()
    {
      //var categories = await _context.Categories.ToListAsync();
      //categories.Insert(0, new Category { 
      //  Id = -1,
      //  Title = "No Parent"
      //});
      ViewData["ParentCategoryId"] = new SelectList(await GetItemsSelectCategories(), "Id", "Title", null);
      //new SelectList(categories, "Id", "Title", -1);
      return View();
    }

    // POST: Admin/Category/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,ParentCategoryId,Title,Content,Slug")] Category category)
    {
      if (ModelState.IsValid)
      {
        if (category.ParentCategoryId == -1)
          category.ParentCategory = null;

        _context.Add(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      //var categories = await _context.Categories.ToListAsync();
      //categories.Insert(0, new Category
      //{
      //  Id = -1,
      //  Title = "No Parent"
      //});
      ViewData["ParentCategoryId"] = new SelectList(await GetItemsSelectCategories(), "Id", "Title", category.ParentCategoryId);
        //new SelectList(categories, "Id", "Title", -1);
      return View(category);
    }

    // GET: Admin/Category/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var category = await _context.Categories.FindAsync(id);
      if (category == null)
      {
        return NotFound();
      }
      ViewData["ParentCategoryId"] = new SelectList(await GetItemsSelectCategories(), "Id", "Title", category.ParentCategoryId);
      //new SelectList(_context.Categories, "Id", "Slug", category.ParentCategoryId);
      return View(category);
    }

    // POST: Admin/Category/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,ParentCategoryId,Title,Content,Slug")] Category category)
    {
      if (id != category.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(category);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!CategoryExists(category.Id))
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
      ViewData["ParentCategoryId"] = new SelectList(await GetItemsSelectCategories(), "Id", "Title", category.ParentCategoryId);
        //new SelectList(_context.Categories, "Id", "Slug", category.ParentCategoryId);
      return View(category);
    }

    // GET: Admin/Category/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var category = await _context.Categories
          .Include(c => c.ParentCategory)
          .FirstOrDefaultAsync(m => m.Id == id);
      if (category == null)
      {
        return NotFound();
      }

      return View(category);
    }

    // POST: Admin/Category/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var category = await _context.Categories.FindAsync(id);
      _context.Categories.Remove(category);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int id)
    {
      return _context.Categories.Any(e => e.Id == id);
    }
  }
}
