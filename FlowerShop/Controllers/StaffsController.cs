using FlowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Controllers
{
    public class StaffsController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext("Data Source=LAPTOP-QCUBM72S;Initial Catalog=QL_CHHoa;User ID=sa;Integrated Security=true");
        // GET: Staffs
        public ActionResult ALL_Account()
        {
            var listsp = db.ACCOUNTs.OrderBy(sp => sp.USERNAME).ToList();
            return View(listsp);
        }

        public ActionResult Add_Account()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add_Account(ACCOUNT model)
        {
            if (ModelState.IsValid)
            {
                ACCOUNT ac = db.ACCOUNTs.FirstOrDefault(x => x.USERNAME.Equals(model.USERNAME));
                if (ac == null)
                {
                    db.ACCOUNTs.InsertOnSubmit(model);
                    db.SubmitChanges();
                    TempData["SuccessMessage"] = "Tạo thành công!";
                    return RedirectToAction("ALL_Account", "Staffs");
                }
                else
                {
                    ViewBag.ErrorMessage = "Tên tài khoản đã tồn tại. Vui lòng nhập tên khác.";
                }

            }
            return View(model);
        }

        public ActionResult Update_Accounts(int id = 0)
        {
            ACCOUNT fl = db.ACCOUNTs.Single(d => d.ID_ACCOUNT == id);
            if (fl == null)
            {
                return HttpNotFound();
            }
            return View(fl);
        }
        [HttpPost]
        public ActionResult Update_Accounts(ACCOUNT model)
        {
            if (ModelState.IsValid)
            {

                ACCOUNT account = db.ACCOUNTs.SingleOrDefault(a => a.ID_ACCOUNT == model.ID_ACCOUNT);


                if (account != null)
                {


                    account.PASSWORDS = model.PASSWORDS;
                    account.ROLES = model.ROLES;


                    db.SubmitChanges();


                    TempData["SuccessMessage"] = "Cập nhật tài khoản thành công!";
                    return RedirectToAction("ALL_Account", "Staffs");
                }
                else
                {

                    ViewBag.ErrorMessage = "Tài khoản không tồn tại.";
                }
            }


            return View(model);
        }

        public ActionResult DeleteAccounts(int id)
        {

            ACCOUNT fl = db.ACCOUNTs.SingleOrDefault(d => d.ID_ACCOUNT == id);
            if (fl == null)
            {
                return HttpNotFound();
            }


            return View(fl);
        }
        [HttpPost, ActionName("DeleteAccounts")]
        public ActionResult DeleteAccountsConfirm(int id)
        {

            ACCOUNT fl = db.ACCOUNTs.SingleOrDefault(d => d.ID_ACCOUNT == id);
            if (fl == null)
            {
                return HttpNotFound();
            }
            var AccountExists = db.STAFFs.Any(fo => fo.ID_ACCOUNT == id);
            if (AccountExists)
            {

                ViewBag.ErrorMessage = "Không thể xóa tài khoản này vì có nhân viên đang sở hữu";
                return View(fl);
            }
            db.ACCOUNTs.DeleteOnSubmit(fl);
            db.SubmitChanges();
            TempData["SuccessMessage"] = "Xóa hoa thành công!";
            return RedirectToAction("ALL_Account", "Staffs");
        }

        public ActionResult All_Staff()
        {
            var listsp = db.STAFFs.OrderBy(sp => sp.FULLNAME).ToList();
            return View(listsp);
        }
        public ActionResult Add_Staffs()
        {
            var accountList = db.ACCOUNTs.Where(a => a.ROLES == "Admin")
                                 .Select(a => new { a.ID_ACCOUNT, a.USERNAME })
                                 .ToList();


            ViewBag.Accounts = new SelectList(accountList, "ID_ACCOUNT", "USERNAME");

            return View();
        }
        [HttpPost]
        public ActionResult Add_Staffs(STAFF model)
        {
            var accountList = db.ACCOUNTs.Where(a => a.ROLES == "Admin")
                         .Select(a => new { a.ID_ACCOUNT, a.USERNAME })
                         .ToList();

            ViewBag.Accounts = new SelectList(accountList, "ID_ACCOUNT", "USERNAME");
            if (ModelState.IsValid)
            {
                STAFF ac = db.STAFFs.FirstOrDefault(x => x.PHONE_NUMBER.Equals(model.PHONE_NUMBER) || x.EMAIL.Equals(model.EMAIL));

                if (ac == null)
                {
                    db.STAFFs.InsertOnSubmit(model);
                    db.SubmitChanges();
                    TempData["SuccessMessage"] = "Tạo thành công!";
                    return RedirectToAction("All_Staff", "Staffs");
                }
                else
                {
                    ViewBag.ErrorMessage = "Số điện thoại hoặc email đã tồn tại. Vui lòng nhập số khác.";
                }

            }
            return View(model);
        }
        public ActionResult Update_Staffs(int id = 0)
        {
            STAFF fl = db.STAFFs.SingleOrDefault(d => d.ID_STAFF == id);
            if (fl == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách tài khoản
            var accountList = db.ACCOUNTs.Select(a => new { a.ID_ACCOUNT, a.USERNAME }).ToList();
            ViewBag.Accounts = new SelectList(accountList, "ID_ACCOUNT", "USERNAME", fl.ID_ACCOUNT);

            return View(fl);
        }

        [HttpPost]

        public ActionResult Update_Staffs(STAFF model)
        {
            // Lấy danh sách tài khoản
            var accountList = db.ACCOUNTs.Select(a => new { a.ID_ACCOUNT, a.USERNAME }).ToList();
            ViewBag.Accounts = new SelectList(accountList, "ID_ACCOUNT", "USERNAME", model.ID_ACCOUNT);

            if (ModelState.IsValid)
            {
                STAFF st = db.STAFFs.SingleOrDefault(a => a.ID_STAFF == model.ID_STAFF);
                if (st != null)
                {
                    st.FULLNAME = model.FULLNAME;
                    st.PHONE_NUMBER = model.PHONE_NUMBER;
                    st.EMAIL = model.EMAIL;
                    st.ID_ACCOUNT = model.ID_ACCOUNT;
                    db.SubmitChanges();
                    TempData["SuccessMessage"] = "Cập nhật nhân viên thành công!";
                    return RedirectToAction("All_Staff", "Staffs");
                }
            }

            return View(model);
        }
        public ActionResult DeleteStaffs(int id)
        {
            // Lấy thông tin nhân viên theo ID
            STAFF staff = db.STAFFs.SingleOrDefault(d => d.ID_STAFF == id);
            if (staff == null)
            {
                return HttpNotFound();
            }

            return View(staff);
        }
        [HttpPost, ActionName("DeleteStaffs")]
        public ActionResult DeleteStaffsConfirm(int id)
        {
            // Lấy thông tin nhân viên theo ID
            STAFF staff = db.STAFFs.SingleOrDefault(d => d.ID_STAFF == id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            db.STAFFs.DeleteOnSubmit(staff);
            db.SubmitChanges();

            TempData["SuccessMessage"] = "Xóa nhân viên thành công!";
            return RedirectToAction("All_Staff", "Staffs");
        }
    }
}