using log4net;
using PetroBM.Domain.Entities;
using PetroBM.Services.Services;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PetroBM.Web.Controllers
{
    public class ImageController : Controller
    {
        private readonly IImageService _imageService;
        private readonly ILog log = LogManager.GetLogger(typeof(ImageController));

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        // GET: Image
        public ActionResult Index()
        {
            return RedirectToAction("ImageList");
        }

        // GET: Image/List
        public ActionResult ImageList()
        {
            try
            {
                var images = _imageService.GetAllImage().ToList();
                return View(images);
            }
            catch (Exception ex)
            {
                log.Error("Error loading image list", ex);
                TempData["AlertMessage"] = "Lỗi khi tải danh sách ảnh.";
                return View(Enumerable.Empty<MImage>());
            }
        }

        // GET: Image/Create
        [HttpGet]
        public ActionResult RegisterImage()
        {
            var model = new MImage
            {
                ImageName = "Sign",
                ImageCode = "Sign",
                ImageURL = "/Content/images/Sign.png",
                ImagePosition = "1,2,3,4",
                ImageUser = "Admin",
                ProcessStatus = 0
            };

            return View(model);
        }

        // POST: Image/Create
        [HttpPost]
        public ActionResult RegisterImage(MImage model)
        {
            if (!ModelState.IsValid)
            {
                TempData["AlertMessage"] = "Thông tin không hợp lệ.";
                return View(model);
            }

            try
            {

                var result = _imageService.CreateImage(model);
                if (result)
                {
                    TempData["AlertMessage"] = "Thêm ảnh thành công.";
                    return RedirectToAction("ImageList");
                }
                else
                {
                    TempData["AlertMessage"] = "Không thể thêm ảnh.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error creating image", ex);
                TempData["AlertMessage"] = "Lỗi khi lưu ảnh.";
                return View(model);
            }
        }

        // GET: Image/Edit/5
        [HttpGet]
        public ActionResult EditImage(int id)
        {
            try
            {
                var model = _imageService.FindImageById(id);
                if (model == null)
                {
                    TempData["AlertMessage"] = "Không tìm thấy ảnh.";
                    return RedirectToAction("ImageList");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                log.Error($"Error loading image with id={id}", ex);
                TempData["AlertMessage"] = "Lỗi khi tải ảnh.";
                return RedirectToAction("ImageList");
            }
        }

        // POST: Image/Edit/5
        [HttpPost]
        public ActionResult EditImage(MImage model)
        {
            if (!ModelState.IsValid)
            {
                TempData["AlertMessage"] = "Dữ liệu không hợp lệ.";
                return View(model);
            }

            try
            {
                var result = _imageService.UpdateImage(model);
                if (result)
                {
                    TempData["AlertMessage"] = "Cập nhật thành công.";
                    return RedirectToAction("ImageList");
                }
                else
                {
                    TempData["AlertMessage"] = "Không thể cập nhật ảnh.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error updating image", ex);
                TempData["AlertMessage"] = "Lỗi khi cập nhật.";
                return View(model);
            }
        }

        // POST: Image/Delete/5
        [HttpPost]
        public ActionResult DeleteImage(int id)
        {
            try
            {
                var result = _imageService.DeleteImage(id);
                if (result)
                {
                    TempData["AlertMessage"] = "Xóa thành công.";
                }
                else
                {
                    TempData["AlertMessage"] = "Không thể xóa ảnh.";
                }
            }
            catch (Exception ex)
            {
                log.Error("Error deleting image", ex);
                TempData["AlertMessage"] = "Lỗi khi xóa.";
            }

            return RedirectToAction("ImageList");
        }

        // GET: Image/View/5
        [HttpGet]
        public ActionResult ViewImage(int id)
        {
            try
            {
                var model = _imageService.FindImageById(id);
                if (model == null)
                {
                    TempData["AlertMessage"] = "Không tìm thấy ảnh.";
                    return RedirectToAction("ImageList");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                //log.Error($"Error loading image view with id={id}", ex);
                log.Error($"Error loading image view with id={id}", ex);
                TempData["AlertMessage"] = "Lỗi khi tải ảnh.";
                return RedirectToAction("ImageList");
            }
        }
    }
}
