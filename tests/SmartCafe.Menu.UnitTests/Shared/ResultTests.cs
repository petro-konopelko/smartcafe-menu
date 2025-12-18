using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.UnitTests.Shared;

public class ResultTests
{
    [Fact]
    public void EnsureError_ReturnsError_OnFailure()
    {
        // Arrange
        var message = "Validation failed";
        var code = "VAL_FAIL";
        var error = Error.Validation(new ErrorDetail(message, code));
        var result = Result.Failure(error);

        // Act
        var ensured = result.EnsureError();

        // Assert
        Assert.Equal(error, ensured);
        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void EnsureValue_ReturnsValue_OnSuccess()
    {
        // Arrange
        var expectedValue = 42;
        var result = Result<int>.Success(expectedValue);

        // Act
        var ensured = result.EnsureValue();

        // Assert
        Assert.Equal(expectedValue, ensured);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void EnsureError_Throws_OnSuccess()
    {
        // Arrange
        var result = Result.Success();
        var expectedMessage = "Cannot retrieve the error";

        // Act + Assert
        var ex = Assert.Throws<InvalidOperationException>(() => result.EnsureError());
        Assert.Contains(expectedMessage, ex.Message);
    }

    [Fact]
    public void EnsureValue_Throws_OnFailure_IncludesErrorDetails()
    {
        // Arrange
        var firstDetailMessage = "Bad field";
        var firstDetailCode = "BAD_FIELD";
        var firstDetailField = "field";
        var secondDetailMessage = "Another issue";
        var secondDetailCode = "ANOTHER";
        var error = Error.Validation(
        [
            new ErrorDetail(firstDetailMessage, firstDetailCode, firstDetailField),
            new ErrorDetail(secondDetailMessage, secondDetailCode)
        ]);
        var result = Result<string>.Failure(error);
        var expectedHeader = "Error type: Validation";

        // Act + Assert
        var ex = Assert.Throws<InvalidOperationException>(() => result.EnsureValue());
        Assert.Contains(expectedHeader, ex.Message);
        Assert.Contains(firstDetailMessage, ex.Message);
        Assert.Contains(secondDetailMessage, ex.Message);
    }

    [Fact]
    public void EnsureValue_Throws_WhenSuccessfulResultContainsNullValue()
    {
        // Arrange
        string? value = null;
        var result = Result<string?>.Success(value);
        var expectedMessage = "Cannot retrieve the value from a failed result";

        // Act + Assert
        var ex = Assert.Throws<InvalidOperationException>(() => result.EnsureValue());
        Assert.Contains(expectedMessage, ex.Message);
    }
}
