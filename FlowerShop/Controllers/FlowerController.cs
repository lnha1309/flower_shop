using FlowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Controllers
{
    public class FlowerController : Controller
    {
       DataClasses1DataContext db = new DataClasses1DataContext("Data Source=LAPTOP-QCUBM72S;Initial Catalog=QL_CHHoa;User ID=sa;Integrated Security=true");
        // GET: Flower
        public ActionResult FlowerDetail(int id)
        {
            var flower = db.FLOWERs.FirstOrDefault(f => f.ID_FLOWER == id);
            var randomFlowers = db.FLOWERs.ToList().OrderBy(f => Guid.NewGuid()).Take(3).ToList();
            ViewBag.RandomFlowers = randomFlowers;
            return View(flower);
        }
    }
}