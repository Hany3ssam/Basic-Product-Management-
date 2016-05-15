using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PearlDev;
using System.IO;

namespace PearlDev.Controllers
{
    public class ProductsController : Controller
    {
        private PearDBEntities db = new PearDBEntities();

        // GET: /Products/
        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            var products = db.Products.Include(p => p.Manufacture);
            //Search feature
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.ProductName.Contains(searchString));
            }
            //Sorting
            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.ProductName);
                    break;
                case "Date":
                    products = products.OrderBy(s => s.ManuDate);
                    break;
                case "date_desc":
                    products = products.OrderByDescending(s => s.ManuDate);
                    break;
                default:
                    products = products.OrderBy(s => s.ProductName);
                    break;
            }
            return View(products.ToList());
        }

        // GET: /Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: /Products/Create
        public ActionResult Create()
        {
            ViewBag.ManufacturerID = new SelectList(db.Manufacture, "ManuID", "ManuName");
            return View();
        }

        // POST: /Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,ManufacturerID,ManuDate,RegistrationDate,,ProductPrice")] Products products, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)

                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        products.ProductImage = reader.ReadBytes(upload.ContentLength);
                    }
                db.Products.Add(products);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ManufacturerID = new SelectList(db.Manufacture, "ManuID", "ManuName", products.ManufacturerID);
            return View(products);
        }

        // GET: /Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            ViewBag.ManufacturerID = new SelectList(db.Manufacture, "ManuID", "ManuName", products.ManufacturerID);
            return View(products);
        }

        // POST: /Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,ManufacturerID,ManuDate,RegistrationDate,ProductPrice")] Products products, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)

                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        products.ProductImage = reader.ReadBytes(upload.ContentLength);
                    }
                db.Entry(products).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ManufacturerID = new SelectList(db.Manufacture, "ManuID", "ManuName", products.ManufacturerID);
            return View(products);
        }

        // GET: /Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: /Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = db.Products.Find(id);
            db.Products.Remove(products);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //Show Image
        public ActionResult showImg(int id)
        {
            var image = (from m in db.Products
                         where m.ProductID == id
                         select m.ProductImage).FirstOrDefault();

            var stream = new MemoryStream(image.ToArray());

            return new FileStreamResult(stream, "image/jpeg");
        }
    }
}
