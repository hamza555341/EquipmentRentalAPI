using EquipmentRental.Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Infrastructure.Persistence.Configurations
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.UserId)
                .IsRequired();

            builder.HasOne<Equipment>()
                .WithMany()
                .HasForeignKey(l => l.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(l => l.BorrowedAt).IsRequired();
            builder.Property(l => l.DueDate).IsRequired();
        }
    }
}