using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020598.BusinessLayers;
using SV20T1020598.DomainModels;
using SV20T1020598.Web;
using SV20T1020598.Web.Models;
using System.Reflection;

namespace SV20T1020598.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    public class EmployeeController : Controller
    {
        const int PAGE_SIZE = 20;
        const string CREATE_TITLE = "Bổ sung nhân viên";
        const string EMPLOYEE_SEARCH = "employee_search";
        public IActionResult Index()
        {
            //Lấy đầu vào tìm kiếm hiện đang lưu lại trong session
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);
            //Trường hợp trong session chưa có điều kiện thì tạo điều kiện mới
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""

                };
            }
            return View(input);
        }

        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new EmployeeSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            //Lưu lại điều kiện tìm kiếm vào trong session
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            var model = new Employee()
            {
                EmployeeID = 0,
                Photo = "nophoto.png",
                BirthDate = new DateTime(1990, 1, 1)
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrWhiteSpace(model.Photo))
                model.Photo = "nophoto.png";
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Employee model, string birthDateInput, IFormFile? uploadPhoto)
        {
            //Xử lý ngày sinh
            DateTime? birthDate = birthDateInput.ToDateTime();
            if (birthDate.HasValue)
                model.BirthDate = birthDate.Value;

            //Xử lý với ảnh upload (nếu có ảnh upload thì lưu ảnh và gán lại tên file ảnh mới cho employee)
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";// Tên file lưu trên server
                                                                                 //Đường dẫn vật lý đến file sẽ lưu lên server
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\employees", fileName);

                //Lưu file lên server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                //Gán tên file ảnh cho model.Photo
                model.Photo = fileName;
            }

           
                //Kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState (nếu có)
                if (string.IsNullOrWhiteSpace(model.FullName))
                    ModelState.AddModelError("FullName", "Tên không được để trống");
                if (string.IsNullOrWhiteSpace(model.Email))
                    ModelState.AddModelError("Email", "Vui lòng nhập email của khách hàng");
                if (string.IsNullOrWhiteSpace(model.Phone))
                    ModelState.AddModelError(nameof(model.Phone), "SĐT không được để trống");

                //Thông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại lỗi hay không
                if (!ModelState.IsValid)
                {
                    ViewBag.Title = model.EmployeeID == 0 ? CREATE_TITLE : "Cập nhật thông tin nhân viên";
                    return View("Edit", model);
                }

                if (model.EmployeeID == 0)
                {
                    int id = CommonDataService.AddEmployee(model);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(model.Email), "Địa chỉ Email bị trùng");
                        return View("Edit", model);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateEmployee(model);
                    if (!result)
                    {
                        ViewBag.Title = "Cập nhật thông tin nhân viên";
                        ModelState.AddModelError(nameof(model.Email), "Địa chỉ email bị trùng");
                        return View("Edit", model);
                    }
                }
                return RedirectToAction("Index");
            
        }

        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}