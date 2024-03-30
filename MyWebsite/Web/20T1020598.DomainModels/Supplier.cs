﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020598.DomainModels
{
    
/// <summary>
/// Nhà cung cấp
/// </summary>
    public class Supplier
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; } = "";
        public string ContactName { get; set; } = "";
        public string Provice { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime? Date { get; set; }
    }
}

