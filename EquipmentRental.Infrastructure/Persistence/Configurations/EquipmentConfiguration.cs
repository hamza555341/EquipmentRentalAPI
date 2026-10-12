using EquipmentRental.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Infrastructure.Persistence.Configurations
{
    public class EquipmentConfiguration
     : IEntityTypeConfiguration<Equipment>
    {
        public void Configure(EntityTypeBuilder<Equipment> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.Description)
                .HasMaxLength(1000);

            builder.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(e => e.CreatedAt)
                .IsRequired();
        }
    }
}
