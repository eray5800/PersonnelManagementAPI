﻿using DAL.Models;

namespace PersonnelManagementAPI.DTO
{
    public class CompanyRequestDTO
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        public Guid EmployeeId { get; set; }


        public string CompanyDocument { get; set; }

        public string PhoneNumber { get; set; }
    }
}
