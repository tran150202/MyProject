namespace SV20T1020598.Web.Models
{
    public class PaginationSearchInput
    {
        /// <summary>
        /// 
        /// </summary>
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public string SearchValue { get; set; } = "";
    }
    public class ProductSearchInput : PaginationSearchInput
    {
        /// <summary>
        /// 
        /// </summary>
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public int CustomerID { get; set; } = 0;

        public int ProvinceID { get; set; } = 0;
    }
}
