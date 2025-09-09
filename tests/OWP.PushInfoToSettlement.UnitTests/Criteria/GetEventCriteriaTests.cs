using OWP.PushInfoToSettlement.Criteria;
using Xunit;

namespace OWP.PushInfoToSettlement.UnitTests.Criteria
{
    public class GetEventCriteriaTests
    {
        [Fact]
        public void GetEventCriteria_GetSettingKey_ReturnsCorrectKey()
        {
            // Arrange
            var criteria = new GetEventCriteria();

            // Act
            var result = criteria.GetSettingKey();

            // Assert
            Assert.Equal("OWP_API_PushMatchRemarkForSettlement", result);
        }

        [Fact]
        public void GetEventCriteria_IsValid_ReturnsTrue()
        {
            // Arrange
            var criteria = new GetEventCriteria();

            // Act
            var result = criteria.IsValid();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetEventCriteria_GetSettingKey_AlwaysReturnsSameValue()
        {
            // Arrange
            var criteria1 = new GetEventCriteria();
            var criteria2 = new GetEventCriteria();

            // Act
            var result1 = criteria1.GetSettingKey();
            var result2 = criteria2.GetSettingKey();

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void GetEventCriteria_IsValid_AlwaysReturnsTrue()
        {
            // Arrange
            var criteria1 = new GetEventCriteria();
            var criteria2 = new GetEventCriteria();

            // Act
            var result1 = criteria1.IsValid();
            var result2 = criteria2.IsValid();

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        [Fact]
        public void GetEventCriteria_GetSettingKey_IsNotNull()
        {
            // Arrange
            var criteria = new GetEventCriteria();

            // Act
            var result = criteria.GetSettingKey();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetEventCriteria_GetSettingKey_IsNotEmpty()
        {
            // Arrange
            var criteria = new GetEventCriteria();

            // Act
            var result = criteria.GetSettingKey();

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetEventCriteria_GetSettingKey_ContainsExpectedKeywords()
        {
            // Arrange
            var criteria = new GetEventCriteria();

            // Act
            var result = criteria.GetSettingKey();

            // Assert
            Assert.Contains("OWP_API", result);
            Assert.Contains("PushMatchRemark", result);
            Assert.Contains("Settlement", result);
        }

        [Fact]
        public void GetEventCriteria_CanBeInstantiated()
        {
            // Act
            var criteria = new GetEventCriteria();

            // Assert
            Assert.NotNull(criteria);
        }

        [Fact]
        public void GetEventCriteria_ImplementsCriteriaBase()
        {
            // Arrange
            var criteria = new GetEventCriteria();

            // Assert
            Assert.IsAssignableFrom<Fanex.Data.Repository.CriteriaBase>(criteria);
        }

        [Fact]
        public void GetEventCriteria_MultipleInstances_ReturnSameValues()
        {
            // Arrange
            var criteria1 = new GetEventCriteria();
            var criteria2 = new GetEventCriteria();
            var criteria3 = new GetEventCriteria();

            // Act
            var key1 = criteria1.GetSettingKey();
            var key2 = criteria2.GetSettingKey();
            var key3 = criteria3.GetSettingKey();

            var valid1 = criteria1.IsValid();
            var valid2 = criteria2.IsValid();
            var valid3 = criteria3.IsValid();

            // Assert
            Assert.Equal(key1, key2);
            Assert.Equal(key2, key3);
            Assert.Equal(valid1, valid2);
            Assert.Equal(valid2, valid3);
        }
    }
}
