using PDFupload.App_Data;
using PDFupload.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PDFupload.Controllers
{
    public class HomeController : Controller
    {
        private readonly PDFappEntities db;

        public HomeController()
        {
            db = new PDFappEntities();
        }

        public ActionResult Index()
        {
            List<UploadFile> filesList = db.PDForders.Select(x => new UploadFile() { 
                Id = x.Id,
                FileName = x.FileName,
                FileContent = x.FileContent,
                Date = x.Date
            }).ToList();

            return View(filesList);
        }

        [HttpPost]
        public JsonResult Index(HttpPostedFileBase file)
        {
            string fileExt = Path.GetExtension(file.FileName).ToUpper();
            if (file != null && fileExt == ".PDF")
            {
                Stream str = file.InputStream;
                BinaryReader br = new BinaryReader(str);

                PDForder order = new PDForder()
                {
                    FileName = file.FileName,
                    FileContent = br.ReadBytes((int)str.Length),
                    Date = DateTime.Now
                };

                db.PDForders.Add(order);
                db.SaveChanges();

                return Json(new { success = true, order.Id }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.FileStatus = "Invalid file format.";
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Checkout(int? id)
        {
            if (id != null)
            {
                SautinSoft.PdfFocus pdfFocus = new SautinSoft.PdfFocus();
                pdfFocus.OpenPdf(UploadFile(id).FileContent);
                TextFileFormat textFormat = new TextFileFormat();
                textFormat.PDFtext = pdfFocus.ToText();

                return View(textFormat);
            }
            else
            {
                return RedirectToAction("Index");
            }       
        }

        [HttpGet]
        public FileResult DownLoadFile(int id)
        {
            UploadFile file = UploadFile(id);

            return File(file.FileContent, "application/pdf", file.FileName);
        }

        [HttpGet]
        public JsonResult GetPDF(int id)
        {
            if (id > 0)
            {
                return Json(new { success = true, file = UploadFile(id) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            
        }

        public UploadFile UploadFile(int? id)
        {
            if (id == null)
                throw new Exception(); 

            return db.PDForders.Where(x => x.Id == id).Select(x => new UploadFile()
            {
                Id = x.Id,
                FileName = x.FileName,
                FileContent = x.FileContent,
                Date = x.Date
            }).SingleOrDefault();
        }
    }
}