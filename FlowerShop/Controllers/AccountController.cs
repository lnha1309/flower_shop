using FlowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Controllers
{
    public class AccountController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext("Data Source=LAPTOP-QCUBM72S;Initial Catalog=QL_CHHoa;User ID=sa;Integrated Security=true");
        // GET: Account
        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var account = db.ACCOUNTs.FirstOrDefault(a => a.USERNAME.Trim() == username && a.PASSWORDS.Trim() == password);
            if (account != null)
            {
                // Lưu thông tin đăng nhập vào Session
                Session["UserID"] = account.ID_ACCOUNT;
                Session["Username"] = account.USERNAME.Trim();
                Session["Role"] = account.ROLES.Trim();
                var staff = db.STAFFs.FirstOrDefault(s => s.ID_ACCOUNT == account.ID_ACCOUNT);

                if (account.ROLES.Equals("Admin"))
                {
                    Session["EmployeeName"] = staff.FULLNAME;
                    return RedirectToAction("All_Flower", "Products");
                }
                else
                    return RedirectToAction("Index", "Home");
            }

            else
            {
                ViewBag.ErrorMessage = "Sai tên đăng nhập hoặc mật khẩu!";
                return View();
            }
        }


        [HttpPost]
        public JsonResult Register(string username, string password, string confirmPassword)
        {
            // Kiểm tra thông tin đăng ký
            if (string.IsNullOrWhiteSpace(username) || username.Length > 20)
            {
                return Json(new { success = false, message = "Tên tài khoản không hợp lệ!" });
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6 || password.Length > 20)
            {
                return Json(new { success = false, message = "Mật khẩu không hợp lệ!" });
            }

            if (password != confirmPassword)
            {
                return Json(new { success = false, message = "Mật khẩu xác nhận không khớp!" });
            }

            // Kiểm tra tên tài khoản đã tồn tại chưa
            var existingUser = db.ACCOUNTs.FirstOrDefault(a => a.USERNAME == username.Trim());
            if (existingUser != null)
            {
                return Json(new { success = false, message = "Tài khoản đã tồn tại!" });
            }

            // Thực hiện đăng ký
            var newAccount = new ACCOUNT
            {
                USERNAME = username.Trim(),
                PASSWORDS = password.Trim(),
                ROLES = "Customer"
            };

            try
            {
                // Thêm tài khoản mới vào bảng ACCOUNT
                db.ACCOUNTs.InsertOnSubmit(newAccount);
                db.SubmitChanges();

                // Tạo bản ghi khách hàng mới trong bảng CUSTOMER
                var newCustomer = new CUSTOMER
                {
                    ID_ACCOUNT = newAccount.ID_ACCOUNT,
                    FULLNAME = "", // Thông tin mặc định, có thể thay đổi sau
                    EMAIL = "",
                    PHONE_NUMBER = "",
                    ADDRESSS = ""
                };

                // Thêm khách hàng vào bảng CUSTOMER
                db.CUSTOMERs.InsertOnSubmit(newCustomer);
                db.SubmitChanges();

                return Json(new { success = true, message = "Đăng ký thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }




        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        // Phương thức GET cho trang đăng ký
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public ActionResult UserInfo()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login");
            }
            int accountId = Convert.ToInt32(Session["UserID"]);
            var customer = db.CUSTOMERs.FirstOrDefault(c => c.ID_ACCOUNT == accountId);
            return View(customer);
        }

        [HttpPost]
        public ActionResult UpdateUserInfo(CUSTOMER customerInfo)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login");
            }
            int accountId = Convert.ToInt32(Session["UserID"]);
            var existingCustomer = db.CUSTOMERs.FirstOrDefault(c => c.ID_ACCOUNT == accountId);
            if (existingCustomer == null)
            {
                // Create new customer record
                existingCustomer = new CUSTOMER
                {
                    ID_ACCOUNT = accountId
                };
                db.CUSTOMERs.InsertOnSubmit(existingCustomer);
            }
            // Update customer information
            existingCustomer.FULLNAME = customerInfo.FULLNAME;
            existingCustomer.EMAIL = customerInfo.EMAIL;
            existingCustomer.PHONE_NUMBER = customerInfo.PHONE_NUMBER;
            existingCustomer.ADDRESSS = customerInfo.ADDRESSS;
            try
            {
                db.SubmitChanges();
                ViewBag.SuccessMessage = "Cập nhật thông tin thành công!";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View("UserInfo", existingCustomer);
        }

        [HttpPost]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmNewPassword)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login");
            }

            int accountId = Convert.ToInt32(Session["UserID"]);

            // Lấy thông tin khách hàng
            var customer = db.CUSTOMERs.FirstOrDefault(c => c.ID_ACCOUNT == accountId);
            var account = db.ACCOUNTs.FirstOrDefault(a => a.ID_ACCOUNT == accountId);

            if (account == null)
            {
                ViewBag.ErrorMessage = "Tài khoản không tồn tại.";
                return View("UserInfo", customer);
            }

            // Kiểm tra mật khẩu hiện tại
            if (account.PASSWORDS.Trim() != currentPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu hiện tại không chính xác.";
                return View("UserInfo", customer);
            }

            // Kiểm tra mật khẩu mới
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ViewBag.ErrorMessage = "Mật khẩu mới không được để trống.";
                return View("UserInfo", customer);
            }

            if (newPassword.Length < 6 || newPassword.Length > 20)
            {
                ViewBag.ErrorMessage = "Mật khẩu mới phải có độ dài từ 6 đến 20 ký tự.";
                return View("UserInfo", customer);
            }

            if (newPassword != confirmNewPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
                return View("UserInfo", customer);
            }

            // Cập nhật mật khẩu
            account.PASSWORDS = newPassword.PadRight(20); // Đảm bảo đúng độ dài CHAR(20)

            try
            {
                db.SubmitChanges();
                ViewBag.SuccessMessage = "Đổi mật khẩu thành công!";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }

            return View("UserInfo", customer);
        }
    }
}