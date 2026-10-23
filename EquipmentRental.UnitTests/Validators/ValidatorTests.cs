using EquipmentRental.Application.Features.Equipment.Commands.CreateEquipment;
using EquipmentRental.Application.Features.Loan.Commands.BorrowEquipment;
using FluentValidation.TestHelper;

namespace EquipmentRental.UnitTests.Validators;

public class CreateEquipmentCommandValidatorTests
{
    private readonly CreateEquipmentCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var result = _validator.TestValidate(new CreateEquipmentCommand("Camera", "Nice camera"));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {
        var result = _validator.TestValidate(new CreateEquipmentCommand("", null));

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsLongerThan150()
    {
        var result = _validator.TestValidate(new CreateEquipmentCommand(new string('a', 151), null));

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldPass_WhenNameIsExactly150()
    {
        var result = _validator.TestValidate(new CreateEquipmentCommand(new string('a', 150), null));

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenDescriptionIsLongerThan1000()
    {
        var result = _validator.TestValidate(new CreateEquipmentCommand("Camera", new string('d', 1001)));

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}

public class BorrowEquipmentCommandValidatorTests
{
    private readonly BorrowEquipmentCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var result = _validator.TestValidate(
            new BorrowEquipmentCommand(1, DateTimeOffset.UtcNow.AddDays(3)));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenEquipmentIdIsZero()
    {
        var result = _validator.TestValidate(
            new BorrowEquipmentCommand(0, DateTimeOffset.UtcNow.AddDays(3)));

        result.ShouldHaveValidationErrorFor(x => x.EquipmentId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenDueDateIsInThePast()
    {
        var result = _validator.TestValidate(
            new BorrowEquipmentCommand(1, DateTimeOffset.UtcNow.AddDays(-1)));

        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }
}
