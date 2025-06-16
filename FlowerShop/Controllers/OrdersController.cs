using FlowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Controllers
{
    public class OrdersController : Controller
    {
        // GET: Orders
        DataClasses1DataContext db = new DataClasses1DataContext("Data Source=LAPTOP-QCUBM72S;Initial Catalog=QL_CHHoa;User ID=sa;Integrated Security=true");
        public ActionResult All_Orders()
        {
            var listsp = db.ORDERs.OrderBy(sp => sp.ORDER_DATE).ToList();
            return View(listsp);
        }
        public ActionResult ALL_ORDERDETAIL(int id)
        {
            var listsp = db.ORDERDETAILs.OrderBy(sp => sp.ID_ORDER == id).ToList();
            return View(listsp);
        }


        public ActionResult Update_Orders(int id = 0)
        {
            ORDER fl = db.ORDERs.Single(d => d.ID_ORDER == id);
            if (fl == null)
            {
                return HttpNotFound();
            }
            return View(fl);
        }
        [HttpPost]
        public ActionResult Update_Orders(ORDER model)
        {
            if (ModelState.IsValid)
            {

                ORDER od = db.ORDERs.SingleOrDefault(a => a.ID_ORDER == model.ID_ORDER);


                if (od != null)
                {
                    od.ORDER_STATUS = model.ORDER_STATUS;
                    db.SubmitChanges();
                    TempData["SuccessMessage"] = "Cập nhật đơn hàng thành công!";
                    return RedirectToAction("All_Orders", "Orders");
                }

            }


            return View(model);
        }
    }
}