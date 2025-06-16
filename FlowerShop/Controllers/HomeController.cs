using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlowerShop.Models;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
namespace FlowerShop.Controllers
{
    public class HomeController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext("Data Source=LAPTOP-QCUBM72S;Initial Catalog=QL_CHHoa;User ID=sa;Integrated Security=true");

        public ActionResult Index()
        {

            var occasions = db.OCCASIONs.ToList();

            var flowersByOccasion = new Dictionary<string, List<FLOWER>>();

            foreach (var occasion in occasions)
            {
                // Lấy danh sách hoa cho mỗi dịp, chỉ lấy 2 hoa
                var flowers = (from f in db.FLOWERs
                               join fo in db.FLOWEROCCASIONs on f.ID_FLOWER equals fo.ID_FLOWER
                               where fo.ID_OCCASION == occasion.ID_OCCASION
                               select f).Take(3).ToList();

                flowersByOccasion[occasion.OCCASION_NAME] = flowers;
            }

            return View(flowersByOccasion);
        }
        public ActionResult Search(string keyword)
        {
            // If keyword is empty or null, return an empty list
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return View(new List<FLOWER>());
            }

            // Search for flowers by name, using case-insensitive partial match
            var searchResults = db.FLOWERs
                .Where(f => f.FLOWER_NAME.ToLower().Contains(keyword.ToLower()))
                .ToList();

            return View(searchResults);
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Subscribe(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["SubscribeMessage"] = "Vui lòng nhập email";
                return RedirectToAction("Index");
            }

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("floryn.flowershop@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Đăng Ký Nhận Thông Tin";
                mail.Body = "Cảm ơn bạn đã đăng ký nhận thông tin ưu đãi";

                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("floryn.flowershop@gmail.com", "szmk emxw tyic ggbr");
                smtpClient.EnableSsl = true;

                smtpClient.Send(mail);

                TempData["SubscribeMessage"] = "Đăng ký thành công! Vui lòng kiểm tra email.";
            }
            catch (Exception ex)
            {
                TempData["SubscribeMessage"] = "Có lỗi xảy ra: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}