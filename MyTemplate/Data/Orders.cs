using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyTemplate.Data
{
  [Table("Orders")]
  public class Orders
  {
    [Key]
    public int Id { set; get; }
    public DateTime CreatedAt { set; get; }
    public string CustomerName { set; get; }

    public ICollection<OrderDetails> Details { set; get; }
  }
}
