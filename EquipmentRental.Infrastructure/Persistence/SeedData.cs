using EquipmentRental.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Infrastructure.Persistence
{


    public class SeedData : IDbIntializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SeedData> _logger;

        public SeedData(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<SeedData> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InitializeDataAsync()
        {
            try
            {
                // 1. Create roles if they don't exist
                string[] roles = { "Admin", "Member" };

                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        var result = await _roleManager.CreateAsync(
                            new IdentityRole(role));

                        if (!result.Succeeded)
                        {
                            var errors = string.Join(
                                ", ",
                                result.Errors.Select(e => e.Description));

                            throw new InvalidOperationException(
                                $"Failed to create role {role}: {errors}");
                        }
                    }
                }

                // 2. Read Admin credentials from configuration
                var email = _configuration["AdminSeed:Email"];
                var password = _configuration["AdminSeed:Password"];

                if (string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(password))
                {
                    throw new InvalidOperationException(
                        "Admin seed credentials are missing from configuration.");
                }

                // 3. Find or create the Admin
                var admin = await _userManager.FindByEmailAsync(email);

                if (admin is null)
                {
                    admin = new IdentityUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(
                        admin, password);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(
                            ", ",
                            result.Errors.Select(e => e.Description));

                        throw new InvalidOperationException(
                            $"Failed to create Admin: {errors}");
                    }
                }

                // 4. Ensure the account has the Admin role
                if (!await _userManager.IsInRoleAsync(admin, "Admin"))
                {
                    var result = await _userManager.AddToRoleAsync(
                        admin, "Admin");

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(
                            ", ",
                            result.Errors.Select(e => e.Description));

                        throw new InvalidOperationException(
                            $"Failed to assign Admin role: {errors}");
                    }
                }

                _logger.LogInformation(
                    "Identity seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while seeding Identity data.");
                throw;
            }
        }
    }

}

