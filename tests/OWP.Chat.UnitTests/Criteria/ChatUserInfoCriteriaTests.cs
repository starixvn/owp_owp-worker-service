using OWP.Chat.Criteria;

namespace OWP.Chat.UnitTests.Criteria
{
    public class ChatUserInfoCriteriaTests
    {
        [Fact]
        public void ChatUserInfoCriteria_DefaultConstructor_InitializesProperties()
        {
            // Act
            var criteria = new ChatUserInfoCriteria();

            // Assert
            Assert.Null(criteria.UserJson);
        }

        [Fact]
        public void ChatUserInfoCriteria_WithValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            var userJson = "{\"UserId\":1,\"ChatUserId\":\"chat123\"}";

            // Act
            var criteria = new ChatUserInfoCriteria
            {
                UserJson = userJson
            };

            // Assert
            Assert.Equal(userJson, criteria.UserJson);
        }

        [Fact]
        public void GetSettingKey_ReturnsCorrectKey()
        {
            // Arrange
            var criteria = new ChatUserInfoCriteria();

            // Act
            var settingKey = criteria.GetSettingKey();

            // Assert
            Assert.Equal("OWP_UpdateChatUserId", settingKey);
        }

        [Fact]
        public void IsValid_AlwaysReturnsTrue()
        {
            // Arrange
            var criteria = new ChatUserInfoCriteria();

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("{\"UserId\":1}")]
        [InlineData("{\"ChatUserId\":\"chat123\"}")]
        [InlineData("{\"UserId\":1,\"ChatUserId\":\"chat123\",\"UserName\":\"user1\"}")]
        [InlineData("invalid json")]
        public void IsValid_WithDifferentUserJsonValues_AlwaysReturnsTrue(string userJson)
        {
            // Arrange
            var criteria = new ChatUserInfoCriteria
            {
                UserJson = userJson
            };

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ChatUserInfoCriteria_PropertiesAreSettable()
        {
            // Arrange
            var criteria = new ChatUserInfoCriteria();

            // Act
            criteria.UserJson = "{\"UserId\":1,\"ChatUserId\":\"chat123\"}";

            // Assert
            Assert.Equal("{\"UserId\":1,\"ChatUserId\":\"chat123\"}", criteria.UserJson);
        }

        [Fact]
        public void ChatUserInfoCriteria_PropertiesCanBeChanged()
        {
            // Arrange
            var criteria = new ChatUserInfoCriteria
            {
                UserJson = "{\"UserId\":1,\"ChatUserId\":\"chat123\"}"
            };

            // Act
            criteria.UserJson = "{\"UserId\":2,\"ChatUserId\":\"chat456\"}";

            // Assert
            Assert.Equal("{\"UserId\":2,\"ChatUserId\":\"chat456\"}", criteria.UserJson);
        }

        [Fact]
        public void ChatUserInfoCriteria_ImplementsINonQueryCommand()
        {
            // Arrange
            var criteria = new ChatUserInfoCriteria();

            // Act & Assert
            Assert.IsAssignableFrom<Fanex.Data.Repository.INonQueryCommand>(criteria);
        }

        [Fact]
        public void IsValid_WithNullUserJson_ReturnsTrue()
        {
            // Arrange
            var criteria = new ChatUserInfoCriteria
            {
                UserJson = null
            };

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
        }
    }
}
