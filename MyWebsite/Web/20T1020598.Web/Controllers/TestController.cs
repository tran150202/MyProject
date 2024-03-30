using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace SV20T1020598.Web.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Create()
        {
            var model = new Models.Person()
            {
                Name = "Hoang Cong Huy",
                Birthday = new DateTime(2002,06,26),
                Salary = 100m
            };
            return View(model);
        }

        public IActionResult Save(Models.Person model, string birthdayInput="") {
            //Chuyển chuỗi thành giá trị ngày, nếu hợp lệ thì mới dùng giá trị do người dùng nhập
            DateTime? d = null;
            try
            {
                d = DateTime.ParseExact(birthdayInput, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {

            }

            if(d.HasValue)
                model.Birthday = d.Value;
            
            return Json(model); 
        }
    }
}
