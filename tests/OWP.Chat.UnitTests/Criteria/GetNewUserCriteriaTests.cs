using OWP.Chat.Criteria;

namespace OWP.Chat.UnitTests.Criteria
{
    public class GetNewUserCriteriaTests
    {
        [Fact]
        public void GetNewUserCriteria_DefaultConstructor_InitializesProperties()
        {
            // Act
            var criteria = new GetNewUserCriteria();

            // Assert
            // No properties to initialize in default constructor
        }

        [Fact]
        public void GetSettingKey_ReturnsCorrectKey()
        {
            // Arrange
            var criteria = new GetNewUserCriteria();

            // Act
            var settingKey = criteria.GetSettingKey();

            // Assert
            Assert.Equal("OWP_GetNewUser", settingKey);
        }

        [Fact]
        public void IsValid_AlwaysReturnsTrue()
        {
            // Arrange
            var criteria = new GetNewUserCriteria();

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void GetNewUserCriteria_ImplementsCriteriaBase()
        {
            // Arrange
            var criteria = new GetNewUserCriteria();

            // Act & Assert
            Assert.IsAssignableFrom<Fanex.Data.Repository.CriteriaBase>(criteria);
        }

        [Fact]
        public void GetNewUserCriteria_CanBeInstantiatedMultipleTimes()
        {
            // Act
            var criteria1 = new GetNewUserCriteria();
            var criteria2 = new GetNewUserCriteria();

            // Assert
            Assert.NotNull(criteria1);
            Assert.NotNull(criteria2);
            Assert.NotSame(criteria1, criteria2);
        }

        [Fact]
        public void GetNewUserCriteria_GetSettingKey_ReturnsSameValueForMultipleInstances()
        {
            // Arrange
            var criteria1 = new GetNewUserCriteria();
            var criteria2 = new GetNewUserCriteria();

            // Act
            var settingKey1 = criteria1.GetSettingKey();
            var settingKey2 = criteria2.GetSettingKey();

            // Assert
            Assert.Equal(settingKey1, settingKey2);
            Assert.Equal("OWP_GetNewUser", settingKey1);
        }

        [Fact]
        public void GetNewUserCriteria_IsValid_ReturnsSameValueForMultipleInstances()
        {
            // Arrange
            var criteria1 = new GetNewUserCriteria();
            var criteria2 = new GetNewUserCriteria();

            // Act
            var isValid1 = criteria1.IsValid();
            var isValid2 = criteria2.IsValid();

            // Assert
            Assert.Equal(isValid1, isValid2);
            Assert.True(isValid1);
        }
    }
}
