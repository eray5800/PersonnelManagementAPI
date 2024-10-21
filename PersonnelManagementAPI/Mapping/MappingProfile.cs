using AutoMapper;
using DAL.Enums;
using DAL.Models;
using Data.Models;
using PersonnelManagementAPI.DTO;

namespace PersonnelManagementAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Employee -> EmployeeDTO
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.EmployeeDetail != null ? src.EmployeeDetail.Address : null))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.EmployeeDetail != null ? src.EmployeeDetail.Position : null))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.EmployeeDetail != null ? src.EmployeeDetail.Department : null))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.EmployeeDetail != null ? src.EmployeeDetail.City : null))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.EmployeeDetail != null ? src.EmployeeDetail.BirthDate : (DateTime?)null))
                .ForMember(dest => dest.Educations, opt => opt.MapFrom(src => src.EmployeeDetail != null ? src.EmployeeDetail.Educations : null))
                .ForMember(dest => dest.Certifications, opt => opt.MapFrom(src => src.EmployeeDetail != null ? src.EmployeeDetail.Certifications : null))
                .ForMember(dest => dest.Experiences, opt => opt.MapFrom(src => src.EmployeeDetail != null ? src.EmployeeDetail.Experiences : null))
                .ForMember(dest => dest.RemainingLeaveDays, opt => opt.MapFrom(src => src.EmployeeDetail != null ? src.EmployeeDetail.RemainingLeaveDays : 0))
                .ForMember(dest => dest.LeaveRequests, opt => opt.MapFrom(src => src.LeaveRequests))
                .ForMember(dest => dest.ExpenseRequests, opt => opt.MapFrom(src => src.ExpenseRequests))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId));

            // SaveEmployeeDTO -> Employee
            CreateMap<SaveEmployeeDTO, Employee>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.EmployeeDetail, opt => opt.Ignore());

            // EmployeeDetail -> EmployeeDTO
            CreateMap<EmployeeDetail, EmployeeDTO>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Educations, opt => opt.MapFrom(src => src.Educations))
                .ForMember(dest => dest.Certifications, opt => opt.MapFrom(src => src.Certifications))
                .ForMember(dest => dest.Experiences, opt => opt.MapFrom(src => src.Experiences))
                .ForMember(dest => dest.RemainingLeaveDays, opt => opt.MapFrom(src => src.RemainingLeaveDays));

            // SaveEmployeeDetailDTO -> EmployeeDetail
            CreateMap<SaveEmployeeDetailDTO, EmployeeDetail>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Educations, opt => opt.MapFrom(src => src.Educations))
                .ForMember(dest => dest.Certifications, opt => opt.MapFrom(src => src.Certifications))
                .ForMember(dest => dest.Experiences, opt => opt.MapFrom(src => src.Experiences));


            // SaveAdminEmployeeDetailDTO -> EmployeeDetail
            CreateMap<SaveAdminEmployeeDetailDTO, EmployeeDetail>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Educations, opt => opt.MapFrom(src => src.Educations))
                .ForMember(dest => dest.Certifications, opt => opt.MapFrom(src => src.Certifications))
                .ForMember(dest => dest.Experiences, opt => opt.MapFrom(src => src.Experiences))
                .ForMember(dest => dest.RemainingLeaveDays, opt => opt.MapFrom(src => src.RemainingLeaveDays));

            // SaveAdminEmployeeDTO -> Employee (For Admin-specific user operations)
            CreateMap<SaveAdminEmployeeDTO, Employee>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));


            // CompanyRequest -> CompanyRequestDTO
            CreateMap<CompanyRequest, CompanyRequestDTO>()
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.CompanyDocument, opt => opt.MapFrom(src => src.CompanyDocument))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            // SaveCompanyRequestDTO -> CompanyRequest
            CreateMap<SaveCompanyRequestDTO, CompanyRequest>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CompanyDocument, opt => opt.MapFrom(src => src.CompanyDocument))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));


            // LeaveRequest -> LeaveRequestDTO
            CreateMap<LeaveRequest, LeaveRequestDTO>();

            // ExpenseRequest -> ExpenseRequestDTO
            CreateMap<ExpenseRequest, ExpenseRequestDTO>();

            // Company -> CompanyDTO
            CreateMap<Company, CompanyDTO>()
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AdminID, opt => opt.MapFrom(src => src.AdminID))
                .ForMember(dest => dest.Employees, opt => opt.MapFrom(src => src.Employees));

            // CompanyDTO -> Company
            CreateMap<CompanyDTO, Company>()
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AdminID, opt => opt.MapFrom(src => src.AdminID));


            // LeaveRequest -> LeaveRequestDTO
            CreateMap<LeaveRequest, LeaveRequestDTO>()
                .ForMember(dest => dest.LeaveRequestId , opt => opt.MapFrom(src => src.LeaveRequestId))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason));

            // SaveLeaveRequestDTO -> LeaveRequest
            CreateMap<SaveLeaveRequestDTO, LeaveRequest>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType)) // Assumes enum to enum mapping
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason));

            // LeaveRequestDTO -> LeaveRequest
            CreateMap<LeaveRequestDTO, LeaveRequest>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => Enum.Parse(typeof(LeaveType), src.LeaveType))) // String to enum conversion
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason));

            // LeaveRequest -> Leave
            CreateMap<LeaveRequest, Leave>()
                .ForMember(dest => dest.LeaveId, opt => opt.Ignore()) 
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company)) 
                .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee));

            // Leave -> LeaveDTO
            CreateMap<Leave, LeaveDTO>()
                .ForMember(dest => dest.LeaveId, opt => opt.MapFrom(src => src.LeaveId))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType.ToString()))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason));

            CreateMap<LeaveDTO, Leave>()
              .ForMember(dest => dest.LeaveId, opt => opt.MapFrom(src => src.LeaveId))
              .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
              .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
              .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
              .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
              .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => Enum.Parse(typeof(LeaveType), src.LeaveType)))
              .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason));

            // ExpenseRequest -> ExpenseRequestDTO
            CreateMap<ExpenseRequest, ExpenseRequestDTO>()
                .ForMember(dest => dest.ExpenseRequestId, opt => opt.MapFrom(src => src.ExpenseRequestId))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ExpenseDocument, opt => opt.MapFrom(src => src.ExpenseDocument))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // SaveExpenseRequestDTO -> ExpenseRequest
            CreateMap<SaveExpenseRequestDTO, ExpenseRequest>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ExpenseDocument, opt => opt.MapFrom(src => src.ExpenseDocument));

            // Expense -> ExpenseDTO
            CreateMap<Expense, ExpenseDTO>()
                .ForMember(dest => dest.ExpenseId, opt => opt.MapFrom(src => src.ExpenseId))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ExpenseDocument, opt => opt.MapFrom(src => src.ExpenseDocument));

            // ExpenseDTO -> Expense
            CreateMap<ExpenseDTO, Expense>()
                .ForMember(dest => dest.ExpenseId, opt => opt.MapFrom(src => src.ExpenseId))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ExpenseDocument, opt => opt.MapFrom(src => src.ExpenseDocument));

            // ExpenseRequest -> Expense
            CreateMap<ExpenseRequest, Expense>()
                .ForMember(dest => dest.ExpenseId, opt => opt.Ignore()) // Ignored because it may not exist when mapping from request
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId ?? Guid.Empty)) // Handle nullable Guid
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ExpenseDocument, opt => opt.MapFrom(src => src.ExpenseDocument))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company))
                .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee));
        }
    }
    }

