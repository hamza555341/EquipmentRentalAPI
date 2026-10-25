using EquipmentRental.Application.Common.Results.Errors;
using EquipmentRental.Application.Features.Loan.Commands.BorrowEquipment;
using EquipmentRental.UnitTests.Helpers;
using EquipmentEntity = EquipmentRental.Domain.Entites.Equipment;
using EquipmentStatus = EquipmentRental.Domain.Entites.EquipmentStatus;
using LoanEntity = EquipmentRental.Domain.Entites.Loan;

namespace EquipmentRental.UnitTests.Features.LoanTests;

public class BorrowEquipmentCommandHandlerTests
{
    private readonly FakeUnitOfWork _uow = new();

    private BorrowEquipmentCommandHandler CreateHandler(string? userId = "user-1")
        => new(_uow, TestData.CurrentUser(userId).Object);

    private static DateTimeOffset InTheFuture => DateTimeOffset.UtcNow.AddDays(7);

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var handler = CreateHandler(userId: null);
        var command = new BorrowEquipmentCommand(1, InTheFuture);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Unauthorized, result.FirstError().Type);
        Assert.Equal(0, _uow.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenEquipmentDoesNotExist()
    {
        // Arrange
        var handler = CreateHandler();
        var command = new BorrowEquipmentCommand(999, InTheFuture);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.FirstError().Type);
        Assert.Equal(0, _uow.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenEquipmentIsUnderMaintenance()
    {
        // Arrange
        var equipment = _uow.Set<EquipmentEntity>()
            .Seed(TestData.NewEquipment(status: EquipmentStatus.Maintenance));
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new BorrowEquipmentCommand(equipment.Id, InTheFuture), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.FirstError().Type);
        Assert.Equal("Equipment.UnderMaintenance", result.FirstError().Code);
        Assert.Empty(_uow.Set<LoanEntity>().Items);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenDueDateIsInThePast()
    {
        // Arrange
        var equipment = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment());
        var handler = CreateHandler();
        var pastDate = DateTimeOffset.UtcNow.AddHours(-1);

        // Act
        var result = await handler.Handle(
            new BorrowEquipmentCommand(equipment.Id, pastDate), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.FirstError().Type);
        Assert.Empty(_uow.Set<LoanEntity>().Items);
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenEquipmentAlreadyHasActiveLoan()
    {
        // Arrange
        var equipment = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment());
        _uow.Set<LoanEntity>().Seed(
            TestData.NewLoan(equipment.Id, "someone-else", dueDate: InTheFuture)); // لسه مترجعش
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new BorrowEquipmentCommand(equipment.Id, InTheFuture), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.FirstError().Type);
        Assert.Equal("Equipment.AlreadyBorrowed", result.FirstError().Code);
        Assert.Single(_uow.Set<LoanEntity>().Items); // مفيش Loan جديد اتضاف
        Assert.Equal(0, _uow.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenPreviousLoanWasAlreadyReturned()
    {
        // Arrange
        var equipment = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment());
        _uow.Set<LoanEntity>().Seed(TestData.NewLoan(
            equipment.Id, "old-user",
            dueDate: DateTimeOffset.UtcNow.AddDays(-5),
            returnedAt: DateTimeOffset.UtcNow.AddDays(-6)));
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new BorrowEquipmentCommand(equipment.Id, InTheFuture), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, _uow.Set<LoanEntity>().Items.Count);
    }

    [Fact]
    public async Task Handle_ShouldCreateLoanForCurrentUser_WhenRequestIsValid()
    {
        // Arrange
        var equipment = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment());
        var handler = CreateHandler(userId: "user-1");
        var dueDate = InTheFuture;
        var before = DateTimeOffset.UtcNow;

        // Act
        var result = await handler.Handle(
            new BorrowEquipmentCommand(equipment.Id, dueDate), CancellationToken.None);

        // Assert
        var after = DateTimeOffset.UtcNow;

        Assert.True(result.IsSuccess);

        var loan = Assert.Single(_uow.Set<LoanEntity>().Items);
        Assert.Equal("user-1", loan.UserId);         
        Assert.Equal(equipment.Id, loan.EquipmentId);
        Assert.Equal(dueDate, loan.DueDate);
        Assert.Null(loan.ReturnedAt);
        Assert.InRange(loan.BorrowedAt, before, after);
        Assert.Equal(1, _uow.SaveChangesCallCount);
    }
}
