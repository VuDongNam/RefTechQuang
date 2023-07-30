using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyTemplate.Data;
using MyTemplate.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTemplate.Controllers
{
  [Route("/product")]
  public class ProductController : Controller
  {
    private readonly ILogger<ProductController> _logger;

    private readonly AppDbContext _context;

    // Key lưu chuỗi json của Cart
    public const string CARTKEY = "cart";


    // Lấy cart từ Session (danh sách CartItem)
    List<CartItem> GetCartItems()
    {

      var session = HttpContext.Session;
      string jsoncart = session.GetString(CARTKEY);
      if (jsoncart != null)
      {
        return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
      }
      return new List<CartItem>();
    }

    // Xóa cart khỏi session
    void ClearCart()
    {
      var session = HttpContext.Session;
      session.Remove(CARTKEY);
    }

    // Lưu Cart (Danh sách CartItem) vào session
    void SaveCartSession(List<CartItem> ls)
    {
      var session = HttpContext.Session;
      string jsoncart = JsonConvert.SerializeObject(ls);
      session.SetString(CARTKEY, jsoncart);
    }

    //
    public ProductController(ILogger<ProductController> logger, AppDbContext context)
    {
      _logger = logger;
      _context = context;
    }

    public async Task<IActionResult> Index()
    {
      return View(await _context.Products.ToListAsync());
    }

    [Route("addcart/{pid:int}", Name = "addcart")]
    public IActionResult AddCart([FromRoute] int pid)
    {

      var product = _context.Products
            .Where(p => p.ProductId == pid)
            .FirstOrDefault();

      if (product == null)
        return NotFound("No Product");

      // Xử lý đưa vào Cart ...
      var cart = GetCartItems();
      var cartitem = cart.Find(p => p.product.ProductId == pid);
      if (cartitem != null)
      {
        // Đã tồn tại, tăng thêm 1
        cartitem.quantity++;
      }
      else
      {
        //  Thêm mới
        cart.Add(new CartItem() { quantity = 1, product = product });
      }

      // Lưu cart vào Session
      SaveCartSession(cart);

      // chuyển đến trang hiển thị cart
      return RedirectToAction(nameof(Cart));
    }
    /// xóa item trong cart
    [Route("removecart/{pid:int}", Name ="removecart")]
    public IActionResult RemoveCart([FromRoute] int pid)
    {
      var cart = GetCartItems();
      var cartitem = cart.Find(p => p.product.ProductId == pid);
      if (cartitem != null)
      {
        // Đã tồn tại, tăng thêm 1
        cart.Remove(cartitem);
      }

      SaveCartSession(cart);
      return RedirectToAction(nameof(Cart));
    }

    // Cập nhật
    [HttpPost]
    [Route("updatecart", Name = "updatecart")]
    public IActionResult UpdateCart([FromForm] int pid, [FromForm] int quantity)
    {
      // Cập nhật Cart thay đổi số lượng quantity ...
      var cart = GetCartItems();
      var cartitem = cart.Find(p => p.product.ProductId == pid);
      if (cartitem != null)
      {
        // Đã tồn tại, tăng thêm 1
        cartitem.quantity = quantity;
      }
      SaveCartSession(cart);
      // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
      return Ok();
    }


    // Hiện thị giỏ hàng
    [Route("cart", Name = "cart")]
    public IActionResult Cart()
    {
      return View(GetCartItems());
    }

    [Route("/checkout")]
    public IActionResult CheckOut()
    {
      // Xử lý khi đặt hàng
      return View();
    }

    // xử lý checkout của method POST
    [HttpPost]
    [Route("/checkout")]
    public async Task<IActionResult> CheckOut(string CustomerName)
    {
      Orders ord = new Orders();
      ord.CreatedAt = DateTime.Now;
      ord.CustomerName = CustomerName;

      var cart = GetCartItems();
      List<OrderDetails> details = new List<OrderDetails>();
      foreach (var item in cart)
      {
        OrderDetails od = new OrderDetails();
        od.Order = ord;
        od.ProductId = item.product.ProductId;
        od.Price = item.product.Price;
        od.Quantity = item.quantity;
      }
      ord.Details = details;
      // lưu ord
      _context.Orders.Add(ord);
      await _context.SaveChangesAsync();
      //clear session
      ClearCart();
      return Redirect(nameof(Index));
    }
  }
}
