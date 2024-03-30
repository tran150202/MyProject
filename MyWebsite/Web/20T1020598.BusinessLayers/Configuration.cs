using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020598.BusinessLayers
{
    
    
    public static class Configuration
    {
        /// <summary>
        /// Chuỗi thông số kết nối với CSDL
        /// </summary>
        public static string ConnectionString { get; private set; } = "";

        /// <summary>
        /// Hàm khởi tạo cấu hình cho BusinessLayer
        /// (Hàm này phải được gọi trước khi chạy ứng dụng)
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString)
        {
            Configuration.ConnectionString = connectionString;
        }
    }
}

// Câu hỏi :
//1. Lớp static là gì? đặc điểm của lớp static là gì?
//2. Hàm , thuộc tính static khác với các hàm/thuộc tính không phải là static ở điểm nào?