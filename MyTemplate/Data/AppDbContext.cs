using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTemplate.Data
{
  public class AppDbContext : IdentityDbContext<AppUser>
  {
    public DbSet<Category> Categories { set; get; }
    public DbSet<Product> Products { set; get; }
    public DbSet<Orders> Orders { set; get; }
    public DbSet<OrderDetails> OrderDetails { set; get; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // phần này sẽ xử lý việc loại bỏ tiền tố AspNet của bảng được sinh ra
    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      // builder.Model.GetEntityTypes(): lấy về các entities 
      foreach (var entityType in builder.Model.GetEntityTypes())
      {
        // lấy về table name trong entity
        string tableName = entityType.GetTableName();
        if (tableName.StartsWith("AspNet"))
        {
          // đổi lại tên table
          entityType.SetTableName(tableName.Substring(6));    // bỏ 6 ký tự đầu là "AspNet"
        }
      }

      // SeedData - chèn ngay bốn sản phẩm khi bảng Product được tạo
      builder.Entity<Product>().HasData(
        new Product()
        {
          ProductId = 1,
          Name = "Product 1",
          Description = "Product 1",
          Price = 10
        },
        new Product()
        {
          ProductId = 2,
          Name = "Product 2",
          Description = "Product 2",
          Price = 15
        },
        new Product()
        {
          ProductId = 3,
          Name = "Product 3",
          Description = "Product 3",
          Price = 50
        },
        new Product()
        {
          ProductId = 4,
          Name = "Product 4",
          Description = "Product 4",
          Price = 45
        }
      );

    }
  }
}
