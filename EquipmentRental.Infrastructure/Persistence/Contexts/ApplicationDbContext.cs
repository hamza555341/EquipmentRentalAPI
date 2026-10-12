using EquipmentRental.Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

public class ApplicationDbContext
    : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

   protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }

    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<Loan> Loans => Set<Loan>();

 
}