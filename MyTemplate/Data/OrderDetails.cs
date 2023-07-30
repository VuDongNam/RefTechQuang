using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyTemplate.Data
{
  [Table("OrderDetails")]
  public class OrderDetails
  {
    public int Id { set; get; }
    public int OrderId { set; get; }
    public int ProductId { set; get; }
    public decimal Price { set; get; }
    public int Quantity { set; get; }
    [ForeignKey("OrderId")]
    public Orders Order { set; get; }
  }
}
