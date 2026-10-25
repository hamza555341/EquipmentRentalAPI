using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Common.Results.Errors;
using Moq;
using EquipmentEntity = EquipmentRental.Domain.Entites.Equipment;
using EquipmentStatus = EquipmentRental.Domain.Entites.EquipmentStatus;
using LoanEntity = EquipmentRental.Domain.Entites.Loan;

namespace EquipmentRental.UnitTests.Helpers;

public static class TestData
{
    public static EquipmentEntity NewEquipment(
        string name = "Camera",
        EquipmentStatus status = EquipmentStatus.active)
        => new()
        {
            Name = name,
            Status = status,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-30)
        };

    public static LoanEntity NewLoan(
        int equipmentId,
        string userId,
        DateTimeOffset dueDate,
        DateTimeOffset? returnedAt = null,
        DateTimeOffset? borrowedAt = null)
        => new()
        {
            EquipmentId = equipmentId,
            UserId = userId,
            BorrowedAt = borrowedAt ?? DateTimeOffset.UtcNow.AddDays(-20),
            DueDate = dueDate,
            ReturnedAt = returnedAt
        };

    public static Mock<ICurrentUserService> CurrentUser(string? userId, bool isAdmin = false)
    {
        var mock = new Mock<ICurrentUserService>();
        mock.Setup(x => x.UserId).Returns(userId);
        mock.Setup(x => x.IsAuthenticated).Returns(userId is not null);
        mock.Setup(x => x.IsInRole("Admin")).Returns(isAdmin);
        return mock;
    }

    public static Error FirstError(this Result result) => result.Errors[0];
}
