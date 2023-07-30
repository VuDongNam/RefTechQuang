using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyTemplate.Data
{
  [Table("Category")]
  public class Category
  {
    [Key]
    
    public int Id { set; get; }
    [Display(Name = "Parent Category")]
    public int? ParentCategoryId { set; get; }

    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    [Display(Name = "Category Name")]
    public string Title { set; get; }

    // Nội dung, thông tin chi tiết về Category
    [DataType(DataType.Text)]
    [Display(Name = "Content")]
    public string Content { set; get; }

    //chuỗi Url
    [Required]
    //[StringLength(100, MinimumLength = 3, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    //[RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Only accept characters [a-z0-9-]")]
    [Display(Name = "Url")]
    public string Slug { set; get; }

    // Các Category con
    public ICollection<Category> CategoryChildren { get; set; }

    [ForeignKey("ParentCategoryId")]
    [Display(Name = "Parent Category")]
    public Category ParentCategory { set; get; }
  }
}
