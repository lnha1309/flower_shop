using FlowerShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        DataClasses1DataContext db = new DataClasses1DataContext("Data Source=LAPTOP-QCUBM72S;Initial Catalog=QL_CHHoa;User ID=sa;Integrated Security=true");
        public ActionResult All_Flower()
        {

            var listsp = db.FLOWERs.OrderBy(sp => sp.FLOWER_NAME).ToList();
            return View(listsp);
        }

        public ActionResult Add_Flower()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add_Flower(FLOWER model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tên hoa đã tồn tại
                FLOWER fl = db.FLOWERs.FirstOrDefault(x => x.FLOWER_NAME.Equals(model.FLOWER_NAME));
                if (fl == null)
                {

                    if (file != null && file.ContentLength > 0)
                    {
                        // Tạo thư mục `images` nếu chưa tồn tại
                        string imagePath = Server.MapPath("~/Images/");
                        if (!Directory.Exists(imagePath))
                        {
                            Directory.CreateDirectory(imagePath);
                        }

                        // Tạo tên file duy nhất
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        string newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

                        // Lưu file vào thư mục
                        string fullPath = Path.Combine(imagePath, newFileName);
                        file.SaveAs(fullPath);

                        // Lưu đường dẫn vào model
                        model.URL_IMAGE = $"{newFileName}";
                    }

                    // Lưu dữ liệu hoa vào cơ sở dữ liệu
                    db.FLOWERs.InsertOnSubmit(model);
                    db.SubmitChanges();

                    TempData["SuccessMessage"] = "Thêm hoa thành công!";
                    return RedirectToAction("All_Flower", "Products");
                }
                else
                {
                    ViewBag.ErrorMessage = "Hoa đã tồn tại. Vui lòng nhập tên khác.";
                }
            }

            return View(model);
        }
        public ActionResult UpdateFlower(int id = 0)
        {
            FLOWER fl = db.FLOWERs.Single(d => d.ID_FLOWER == id);
            if (fl == null)
            {
                return HttpNotFound();
            }
            return View(fl);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateFlower(FLOWER model)
        {
            if (ModelState.IsValid)
            {
                FLOWER fl = db.FLOWERs.SingleOrDefault(f => f.ID_FLOWER == model.ID_FLOWER);
                if (fl != null)
                {

                    fl.FLOWER_NAME = model.FLOWER_NAME;
                    fl.DESCRIPTIONS = model.DESCRIPTIONS;
                    fl.PRICE = model.PRICE;
                    fl.STOCK = model.STOCK;
                    fl.URL_IMAGE = model.URL_IMAGE;

                    db.SubmitChanges();
                    TempData["SuccessMessage"] = "Cập nhật hoa thành công!";
                    return RedirectToAction("All_Flower", "Products");

                }
            }
            return View(model);
        }
        public ActionResult DetailFlower(int id = 0)
        {
            FLOWER fl = db.FLOWERs.Single(d => d.ID_FLOWER == id);


            if (fl == null)
            {
                return HttpNotFound();
            }
            return View(fl);
        }
        public ActionResult DeleteFlower(int id)
        {

            FLOWER fl = db.FLOWERs.SingleOrDefault(d => d.ID_FLOWER == id);
            if (fl == null)
            {
                return HttpNotFound();
            }

            return View(fl);
        }
        [HttpPost, ActionName("DeleteFlower")]
        public ActionResult DeleteFlowerConfirm(int id)
        {

            FLOWER fl = db.FLOWERs.SingleOrDefault(d => d.ID_FLOWER == id);
            if (fl == null)
            {
                return HttpNotFound();
            }
            var flowerOccasionExists = db.FLOWEROCCASIONs.Any(fo => fo.ID_FLOWER == id);
            if (flowerOccasionExists)
            {

                ViewBag.ErrorMessage = "Không thể xóa hoa này vì hoa này đang tồn tại trong dịp lễ nào đó";
                return View(fl);
            }
            db.FLOWERs.DeleteOnSubmit(fl);
            db.SubmitChanges();
            TempData["SuccessMessage"] = "Xóa hoa thành công!";
            return RedirectToAction("All_Flower", "Products");
        }
        public ActionResult SearchFlower(string keyword)
        {
            var listsp = db.FLOWERs
                           .Where(f => f.FLOWER_NAME.Contains(keyword) ||
                                       f.DESCRIPTIONS.Contains(keyword))
                           .OrderBy(f => f.FLOWER_NAME)
                           .ToList();

            if (!listsp.Any())
            {
                ViewBag.Message = "Không tìm thấy hoa phù hợp với từ khóa.";
            }

            ViewBag.Keyword = keyword;
            return View("All_Flower", listsp);
        }
    }
}