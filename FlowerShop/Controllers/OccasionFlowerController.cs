using FlowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Controllers
{
    public class OccasionFlowerController : Controller
    {
        // GET: OccasionFlower
        DataClasses1DataContext db = new DataClasses1DataContext("Data Source=LAPTOP-QCUBM72S;Initial Catalog=QL_CHHoa;User ID=sa;Integrated Security=true");

        public ActionResult ProductList()
        {
            var flowersByOccasion = new Dictionary<string, List<FLOWER>>();
            var occasions = db.OCCASIONs.ToList();

            foreach (var occasion in occasions)
            {
                var flowers = (from f in db.FLOWERs
                               join fo in db.FLOWEROCCASIONs on f.ID_FLOWER equals fo.ID_FLOWER
                               where fo.ID_OCCASION == occasion.ID_OCCASION
                               select f).ToList();

                flowersByOccasion[occasion.OCCASION_NAME] = flowers;
            }

            ViewBag.Occasions = occasions;
            return View(flowersByOccasion.AsEnumerable());
        }
    }
}