using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020598.BusinessLayers;
using SV20T1020598.DomainModels;
using SV20T1020598.Web.Models;

namespace SV20T1020598.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]

    public class ProductController : Controller
    {
        const int PAGE_SIZE = 20;
        const string PRODUCT_SEARCH = "Product_search";
        public IActionResult Index()
        {
            PaginationSearchInput? input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                };
            }
            return View(input);
        }

        public IActionResult Search(ProductSearchInput input)
        {
            int rowCount = 0;

            var data = ProductDataService.ListProducts(out rowCount, input.Page, PAGE_SIZE,
             input.SearchValue ?? "", input.CategoryID, input.SupplierID
             );
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Categories = CommonDataService.ListOfCategories(out rowCount, 1, PAGE_SIZE, ""),
                Suppliers = CommonDataService.ListOfSupplier(out rowCount, 1, PAGE_SIZE, ""),
                Data = data,
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            ViewBag.IsEdit = false;
            Product model = new Product()
            {
                ProductID = 0,
                Photo = "nophoto.png"
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            ViewBag.IsEdit = true;

            Product? model = ProductDataService.GetProduct(id);
            if (model == null)
            {
                //về lại trang chủ
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(model.Photo))
                model.Photo = "nophoto.png";

            //kiểu customer
            return View(model);
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var model = ProductDataService.GetProduct(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !ProductDataService.InUsedProduct(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Product model, IFormFile? uploadPhoto)
        {


            if (string.IsNullOrWhiteSpace(model.ProductName))
            {
                ModelState.AddModelError(nameof(model.ProductName), "Tên không được để trống!");
            }
            if (string.IsNullOrWhiteSpace(model.Unit))
            {
                ModelState.AddModelError(nameof(model.Unit), "Đơn vị tính không được để trống!");
            }
            if (model.Price == 0)
            {
                ModelState.AddModelError(nameof(model.Price), "Giá hàng không được bằng 0!");
            }
            if (model.CategoryID == 0)
            {
                ModelState.AddModelError(nameof(model.CategoryID), "Loại hàng không được để trống!");
            }
            if (model.SupplierID == 0)
            {
                ModelState.AddModelError(nameof(model.SupplierID), "Nhà cung cấp không được để trống!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.ProductID == 0 ? "Bổ sung khách hàng" : "Cập nhật khách hàng";
                ViewBag.IsEdit = false;
                return View("Edit", model);
            }

            if (uploadPhoto != null)
            {
                //tránh việc trùng tên file nên thêm time trước tên
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                model.Photo = fileName;
            }
            if (model.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(model);

            }
            else
            {
                bool result = ProductDataService.UpdateProduct(model);
            }
            return RedirectToAction("Index");

        }
        public IActionResult SavePhoto(ProductPhoto model, IFormFile? uploadPhoto)
        {
            if (uploadPhoto != null)
            {
                //tránh việc trùng tên file nên thêm time trước tên
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                model.Photo = fileName;
            }

            if (model.Photo.Equals("nophoto.png"))
            {
                ModelState.AddModelError(nameof(model.Photo), "Ảnh không được để trống!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.PhotoID == 0 ? "Bổ sung ảnh" : "Cập nhật ảnh";
                return View("Photo", model);
            }
            if (model.PhotoID == 0)
            {
                long id = ProductDataService.AddPhoto(model);

            }
            else
            {
                bool result = ProductDataService.UpdatePhoto(model);

            }


            return RedirectToAction("Edit", new { id = model.ProductID });
        }
        public IActionResult SaveAttribute(ProductAttribute model)
        {

            ViewBag.Title = model.AttributeID == 0 ? "Bổ sung thuộc tính" : "Cập nhật thuộc tính";

            if (string.IsNullOrWhiteSpace(model.AttributeName))
            {
                ModelState.AddModelError(nameof(model.AttributeName), "Tên thuộc tính không được để trống!");
            }
            if (string.IsNullOrWhiteSpace(model.AttributeValue))
            {
                ModelState.AddModelError(nameof(model.AttributeValue), "Giá trị thuộc tính không được để trống!");
            }

            if (!ModelState.IsValid)
            {

                return View("Attribute", model);
            }
            if (model.AttributeID == 0)
            {
                long id = ProductDataService.AddAttribute(model);

            }
            else
            {
                bool result = ProductDataService.UpdateAttribute(model);

            }

            return RedirectToAction("Edit", new { id = model.ProductID });
        }
        public IActionResult Photo(int id, string method, int photoId = 0)
        {
            var model = new ProductPhoto();
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung Ảnh";
                    model = new ProductPhoto
                    {
                        PhotoID = 0,
                        ProductID = id,
                        Photo = "nophoto.png"
                    };

                    return View("Photo", model);
                case "edit":
                    ViewBag.Title = "Thay đổi Ảnh";
                    model = ProductDataService.GetPhoto(photoId);
                    if (model == null) return RedirectToAction("Index");
                    return View("Photo", model);
                case "delete":
                    //TODO: Xóa ảnh (xóa trực tiếp, không hỏi lại)
                    ProductDataService.DeletePhoto(photoId);
                    ViewBag.IsEdit = true;
                    return View("Edit", ProductDataService.GetProduct(id));
                default:
                    return RedirectToAction("Index");
            }
        }
        public IActionResult Attribute(int id, string method, int attributeId = 0)
        {
            var model = new ProductAttribute();
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    model = new ProductAttribute
                    {
                        AttributeID = 0,
                        ProductID = id,
                    };

                    return View("Attribute", model);
                case "edit":
                    ViewBag.Title = "Cập nhật thuộc tính";
                    model = ProductDataService.GetAttribute(attributeId);
                    if (model == null) return RedirectToAction("Index");
                    return View("Attribute", model);
                case "delete":
                    //TODO : Xoá thuộc tính (xoá trực tiếp)
                    ProductDataService.DeleteAttribute(attributeId);

                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
    }
}
