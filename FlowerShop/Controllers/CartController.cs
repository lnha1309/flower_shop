using FlowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
namespace FlowerShop.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        DataClasses1DataContext db = new DataClasses1DataContext("Data Source=LAPTOP-QCUBM72S;Initial Catalog=QL_CHHoa;User ID=sa;Integrated Security=true");

        // Khi vào trang chủ hoặc bất kỳ trang nào
        private void LoadCartFromSession()
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart; // Nếu giỏ hàng chưa có, tạo mới
            }
            ViewBag.CartItemCount = cart.Sum(c => c.Quantity); // Cập nhật số lượng giỏ hàng
        }

        // GET: Cart
        private void UpdateCartItemCount()
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            ViewBag.CartItemCount = cart.Sum(c => c.Quantity); // Tổng số lượng
        }

        // GET: Cart
        public ActionResult CartDetail()
        {
            // Lấy giỏ hàng từ Session
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            UpdateCartItemCount(); // Cập nhật ViewBag.CartItemCount
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ hàng
        public ActionResult AddToCart(int id)
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            // Tìm sản phẩm theo ID
            var flower = db.FLOWERs.FirstOrDefault(f => f.ID_FLOWER == id);
            if (flower != null)
            {
                var cartItem = cart.FirstOrDefault(c => c.Flower.ID_FLOWER == id);
                if (cartItem != null)
                {
                    cartItem.Quantity++;
                }
                else
                {
                    cart.Add(new CartItem { Flower = flower, Quantity = 1 });
                }
            }

            // Lưu giỏ hàng vào Session
            Session["Cart"] = cart;

            UpdateCartItemCount(); // Cập nhật ViewBag.CartItemCount
            return RedirectToAction("CartDetail");
        }

        [HttpPost]
        public ActionResult UpdateCart(FormCollection form)
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart != null)
            {
                // Duyệt qua tất cả các sản phẩm trong giỏ hàng và cập nhật số lượng
                foreach (var item in cart)
                {
                    // Lấy số lượng từ form gửi lên
                    int quantity = Convert.ToInt32(form["quantity_" + item.Flower.ID_FLOWER]);
                    item.Quantity = quantity > 0 ? quantity : 1;
                }

                // Lưu giỏ hàng vào Session
                Session["Cart"] = cart;
            }

            UpdateCartItemCount(); // Cập nhật ViewBag.CartItemCount
            return RedirectToAction("CartDetail");  
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public ActionResult RemoveFromCart(int id)
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart != null)
            {
                cart.RemoveAll(c => c.Flower.ID_FLOWER == id);
                Session["Cart"] = cart;
            }

            UpdateCartItemCount(); // Cập nhật ViewBag.CartItemCount
            return RedirectToAction("CartDetail");
        }
        [HttpPost]
        public ActionResult Checkout()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("CartDetail");
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            var customer = db.CUSTOMERs.FirstOrDefault(c => c.ID_ACCOUNT == userId);

            // Check if customer profile is incomplete
            if (customer == null ||
                string.IsNullOrWhiteSpace(customer.FULLNAME) ||
                string.IsNullOrWhiteSpace(customer.EMAIL) ||
                string.IsNullOrWhiteSpace(customer.PHONE_NUMBER) ||
                string.IsNullOrWhiteSpace(customer.ADDRESSS))
            {
                // Set a TempData flag to show a message on UserInfo page
                TempData["CheckoutRequiresProfileUpdate"] = true;
                return RedirectToAction("UserInfo", "Account");
            }

            using (var scope = new System.Transactions.TransactionScope())
            {
                try
                {
                    // Rest of the existing checkout logic remains the same
                    var order = new ORDER
                    {
                        ID_CUSTOMER = customer.ID_CUSTOMER,
                        ORDER_DATE = DateTime.Now,
                        TOTAL_AMOUNT = cart.Sum(c => c.Flower.PRICE * c.Quantity),
                        ORDER_STATUS = "Đang xử lí"
                    };

                    db.ORDERs.InsertOnSubmit(order);
                    db.SubmitChanges();

                    // Thêm chi tiết đơn hàng
                    foreach (var item in cart)
                    {
                        var orderDetail = new ORDERDETAIL
                        {
                            ID_ORDER = order.ID_ORDER,
                            ID_FLOWER = item.Flower.ID_FLOWER,
                            QUANTITY = item.Quantity,
                            PRICE = item.Flower.PRICE
                        };

                        db.ORDERDETAILs.InsertOnSubmit(orderDetail);
                    }

                    db.SubmitChanges();
                    scope.Complete();

                    Session["Cart"] = null;
                    UpdateCartItemCount();
                    return RedirectToAction("OrderHistory");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Có lỗi xảy ra trong quá trình thanh toán: " + ex.Message;
                    return RedirectToAction("CartDetail");
                }
            }
        }

        public ActionResult OrderHistory()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            // Lấy danh sách đơn hàng của người dùng
            var orderHistory = (from account in db.ACCOUNTs
                                join customer in db.CUSTOMERs
                                on account.ID_ACCOUNT equals customer.ID_ACCOUNT
                                join order in db.ORDERs
                                on customer.ID_CUSTOMER equals order.ID_CUSTOMER
                                where account.ID_ACCOUNT == userId // So sánh userId với ID_ACCOUNT
                                select new OrderHistoryViewModel
                                {
                                    OrderId = order.ID_ORDER,
                                    OrderDate = order.ORDER_DATE ?? DateTime.Now,
                                    TotalAmount = order.TOTAL_AMOUNT ?? 0m,
                                    OrderStatus = order.ORDER_STATUS,
                                    OrderDetails = order.ORDERDETAILs.Select(od => new OrderDetailViewModel
                                    {
                                        FlowerName = od.FLOWER.FLOWER_NAME,
                                        ImageUrl = od.FLOWER.URL_IMAGE,
                                        Quantity = od.QUANTITY ?? 0,
                                        Price = od.PRICE ?? 0m
                                    }).ToList()
                                }).ToList();


            return View(orderHistory);
        }

    }

    // Lớp hỗ trợ để lưu thông tin giỏ hàng
    public class CartItem
    {
        public FLOWER Flower { get; set; }
        public int Quantity { get; set; }
    }
    
}
