using DAL.Models;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configuration
{
    public class CompanyRequestConfiguration : IEntityTypeConfiguration<CompanyRequest>
    {
        public void Configure(EntityTypeBuilder<CompanyRequest> builder)
        {
            builder.ToTable("CompanyRequests");

            builder.HasKey(cr => cr.CompanyId);

            builder.Property(cr => cr.CompanyName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(cr => cr.Address)
                   .HasMaxLength(200);

            builder.Property(cr => cr.Email)
                   .HasMaxLength(100);

            builder.Property(cr => cr.PhoneNumber)
                   .HasMaxLength(20);

            builder.Property(cr => cr.CompanyDocument)
                   .HasMaxLength(255);


            builder.HasOne(cr => cr.Employee)        
                   .WithMany()                      
                   .HasForeignKey(cr => cr.EmployeeId) 
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
