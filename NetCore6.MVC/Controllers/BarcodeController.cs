using Microsoft.AspNetCore.Mvc;
using NetCore6.MVC.Models;
using System.DrawingCore;
using ZXing.ZKWeb;

namespace NetCore6.MVC.Controllers
{
    public class BarcodeController : Controller
    {
        [HttpGet]
        public IActionResult Reader()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Reader(IFormFile file)
        {
            try
            {
                if (file!=null)
                {
                    var path = await ImageUploadAsync(file);
                    var barcodeReader = new BarcodeReader();
                    var barcodeBitmap = (Bitmap)Bitmap.FromFile(path.FullPath);
                    var barcodeResult = barcodeReader.Decode(barcodeBitmap);
                    ViewBag.Barcode = barcodeResult.Text.ToString();
                    ViewBag.BarcodeUrl = path.FileName;
                }
                else
                {
                    ViewBag.Barcode = "Barkod Resmi Ekleyiniz!";
                    ViewBag.BarcodeUrl = "";
                }
            }
            catch (Exception)
            {
                ViewBag.Barcode = "Barkod Okunamadı!";
                ViewBag.BarcodeUrl = "";
            }
            return View();
        }
        private async Task<ImageInfo> ImageUploadAsync(IFormFile file)
        {
            #region ImageUpload
            string path = "";
            string newFileName = DateTime.Now.ToString("yyyy_MM_dd_HHmmssfff") + "-"+file.FileName;
            try
            {
                if (file.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot/Upload"));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (var fileStream = new FileStream(Path.Combine(path, newFileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return new ImageInfo { FullPath = path + @"\" + newFileName, FileName = "/Upload/" + newFileName };
                }
                else
                {
                    return new ImageInfo();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
            #endregion
        }
    }
}
