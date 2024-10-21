using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.HelperModels
{
    public class CompanyDashboardStats
    {
        public int ExpenseCount { get; set; }
        public decimal TotalExpenses { get; set; }
        public int LeaveRequestCount { get; set; }
        public int ExpenseRequestCount { get; set; }
    }
}
