using AutoMapper;
using DAL.Models;
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

            // SaveAdminEmployeeDTO -> Employee (For Admin-specific user operations)
            CreateMap<SaveAdminEmployeeDTO, Employee>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

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
        }
    }
}
