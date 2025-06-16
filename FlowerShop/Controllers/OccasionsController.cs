using FlowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Controllers
{
    public class OccasionsController : Controller
    {
        // GET: Occasions
        DataClasses1DataContext db = new DataClasses1DataContext("Data Source=LAPTOP-QCUBM72S;Initial Catalog=QL_CHHoa;User ID=sa;Integrated Security=true");
        public ActionResult All_OCCASIONS()
        {
            var list = db.OCCASIONs.OrderBy(sp => sp.OCCASION_NAME).ToList();
            return View(list);
        }

        public ActionResult ADD_OCCASIONS()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ADD_OCCASIONS(OCCASION model)
        {
            if (ModelState.IsValid)
            {
                OCCASION fl = db.OCCASIONs.FirstOrDefault(x => x.OCCASION_NAME.Equals(model.OCCASION_NAME));
                if (fl == null)
                {
                    db.OCCASIONs.InsertOnSubmit(model);
                    db.SubmitChanges();
                    TempData["SuccessMessage"] = "Thêm dịp thành công!";
                    return RedirectToAction("All_OCCASIONS", "Occasions");
                }
                else
                {
                    ViewBag.ErrorMessage = "Dịp đã tồn tại. Vui lòng nhập tên khác.";
                }

            }
            return View(model);
        }

        public ActionResult UPDATE_OCCASIONS(int id = 0)
        {
            OCCASION fl = db.OCCASIONs.Single(d => d.ID_OCCASION == id);
            if (fl == null)
            {
                return HttpNotFound();
            }
            return View(fl);
        }
        [HttpPost]
        public ActionResult UPDATE_OCCASIONS(OCCASION model)
        {
            if (ModelState.IsValid)
            {
                OCCASION fl = db.OCCASIONs.SingleOrDefault(f => f.ID_OCCASION == model.ID_OCCASION);
                if (fl != null)
                {

                    fl.OCCASION_NAME = model.OCCASION_NAME;


                    db.SubmitChanges();
                    TempData["SuccessMessage"] = "Cập nhật dịp thành công!";
                    return RedirectToAction("All_OCCASIONS", "Occasions");

                }
            }
            return View(model);
        }

        public ActionResult DELETE_OCCASIONS(int id)
        {

            OCCASION fl = db.OCCASIONs.SingleOrDefault(d => d.ID_OCCASION == id);
            if (fl == null)
            {
                return HttpNotFound();
            }

            return View(fl);
        }
        [HttpPost, ActionName("DELETE_OCCASIONS")]
        public ActionResult DeleteOccasionsConfirm(int id)
        {

            OCCASION fl = db.OCCASIONs.SingleOrDefault(d => d.ID_OCCASION == id);
            if (fl == null)
            {
                return HttpNotFound();
            }
            var flowerOccasionExists = db.FLOWEROCCASIONs.Any(fo => fo.ID_OCCASION == id);
            if (flowerOccasionExists)
            {

                ViewBag.ErrorMessage = "Không thể xóa dịp này vì dịp này đang tồn tại hoa";
                return View(fl);
            }
            db.OCCASIONs.DeleteOnSubmit(fl);
            db.SubmitChanges();
            TempData["SuccessMessage"] = "Xóa dịp thành công!";
            return RedirectToAction("All_OCCASIONS", "Occasions");
        }


        public ActionResult Details_Occasions(int id)
        {
            OCCASION occasion = db.OCCASIONs.SingleOrDefault(o => o.ID_OCCASION == id);


            if (occasion == null)
            {
                return HttpNotFound();
            }


            var flowers = db.FLOWERs
                            .Join(db.FLOWEROCCASIONs,
                                  f => f.ID_FLOWER,
                                  fo => fo.ID_FLOWER,
                                  (f, fo) => new { f, fo })
                            .Where(x => x.fo.ID_OCCASION == id)
                            .Select(x => x.f)
                            .ToList();
            ViewBag.OccasionId = id;
            return View(flowers);
        }


        public ActionResult AddFlowerToOccasion()
        {
            ViewBag.Flowers = new SelectList(db.FLOWERs.ToList(), "ID_FLOWER", "FLOWER_NAME");
            ViewBag.Occasions = new SelectList(db.OCCASIONs.ToList(), "ID_OCCASION", "OCCASION_NAME");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFlowerToOccasion(int ID_FLOWER, int ID_OCCASION)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem đã tồn tại chưa
                var existing = db.FLOWEROCCASIONs.SingleOrDefault(f => f.ID_FLOWER == ID_FLOWER && f.ID_OCCASION == ID_OCCASION);
                if (existing == null)
                {
                    FLOWEROCCASION newEntry = new FLOWEROCCASION
                    {
                        ID_FLOWER = ID_FLOWER,
                        ID_OCCASION = ID_OCCASION
                    };
                    db.FLOWEROCCASIONs.InsertOnSubmit(newEntry);
                    db.SubmitChanges();

                    TempData["SuccessMessage"] = "Thêm hoa vào dịp thành công!";
                    return RedirectToAction("All_OCCASIONS", "Occasions");
                }
                else
                {
                    ViewBag.ErrorMessage = "Hoa đã được thêm vào dịp này trước đó.";
                }
            }


            ViewBag.Flowers = new SelectList(db.FLOWERs.ToList(), "ID_FLOWER", "FLOWER_NAME");
            ViewBag.Occasions = new SelectList(db.OCCASIONs.ToList(), "ID_OCCASION", "OCCASION_NAME");
            return View();
        }
        public ActionResult DeleteFlowerFromOccasion(int flowerId, int occasionId)
        {
            FLOWEROCCASION fl = db.FLOWEROCCASIONs.SingleOrDefault(f => f.ID_FLOWER == flowerId && f.ID_OCCASION == occasionId);
            if (fl == null)
            {
                return HttpNotFound();
            }

            return View(fl);
        }
        [HttpPost, ActionName("DeleteFlowerFromOccasion")]
        public ActionResult DeleteFlowerFromOccasionConfirm(int flowerId, int occasionId)
        {

            FLOWEROCCASION fl = db.FLOWEROCCASIONs.SingleOrDefault(f => f.ID_FLOWER == flowerId && f.ID_OCCASION == occasionId);
            if (fl == null)
            {
                return HttpNotFound();
            }

            db.FLOWEROCCASIONs.DeleteOnSubmit(fl);
            db.SubmitChanges();
            TempData["SuccessMessage"] = "Xóa dịp thành công!";
            return RedirectToAction("All_OCCASIONS", "Occasions");
        }
    }
}