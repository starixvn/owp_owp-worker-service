using OWP.AutoGenerateMaxbetSetting.Constants;

namespace OWP.AutoGenerateMaxbetSetting.UnitTests.Constants;

public class BettypeTests
{
    [Fact]
    public void Hdp_ShouldReturnCorrectValue()
    {
        // Act & Assert
        Bettype.Hdp.Should().Be(1);
    }

    [Fact]
    public void Ou_ShouldReturnCorrectValue()
    {
        // Act & Assert
        Bettype.Ou.Should().Be(3);
    }

    [Fact]
    public void Hdp1H_ShouldReturnCorrectValue()
    {
        // Act & Assert
        Bettype.Hdp1H.Should().Be(7);
    }

    [Fact]
    public void Ou1H_ShouldReturnCorrectValue()
    {
        // Act & Assert
        Bettype.Ou1H.Should().Be(8);
    }

    [Fact]
    public void AllConstants_ShouldBeUnique()
    {
        // Arrange
        var constants = new[] { Bettype.Hdp, Bettype.Ou, Bettype.Hdp1H, Bettype.Ou1H };

        // Act & Assert
        constants.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void AllConstants_ShouldBePositive()
    {
        // Act & Assert
        Bettype.Hdp.Should().BePositive();
        Bettype.Ou.Should().BePositive();
        Bettype.Hdp1H.Should().BePositive();
        Bettype.Ou1H.Should().BePositive();
    }

    [Fact]
    public void AllConstants_ShouldBeIntegers()
    {
        // Act & Assert
        Bettype.Hdp.Should().BeOfType(typeof(int));
        Bettype.Ou.Should().BeOfType(typeof(int));
        Bettype.Hdp1H.Should().BeOfType(typeof(int));
        Bettype.Ou1H.Should().BeOfType(typeof(int));
    }
}
