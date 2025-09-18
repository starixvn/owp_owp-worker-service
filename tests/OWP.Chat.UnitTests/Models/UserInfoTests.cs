using OWP.Chat.Models;

namespace OWP.Chat.UnitTests.Models
{
    public class UserInfoTests
    {
        [Fact]
        public void UserInfo_DefaultConstructor_InitializesProperties()
        {
            // Act
            var userInfo = new UserInfo();

            // Assert
            Assert.Equal(0, userInfo.UserId);
            Assert.Null(userInfo.UserName);
        }

        [Fact]
        public void UserInfo_WithValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            var userId = 123;
            var userName = "testuser";

            // Act
            var userInfo = new UserInfo
            {
                UserId = userId,
                UserName = userName
            };

            // Assert
            Assert.Equal(userId, userInfo.UserId);
            Assert.Equal(userName, userInfo.UserName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(999999)]
        [InlineData(-1)]
        public void UserInfo_WithDifferentUserId_SetsUserIdCorrectly(int userId)
        {
            // Act
            var userInfo = new UserInfo { UserId = userId };

            // Assert
            Assert.Equal(userId, userInfo.UserId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("testuser")]
        [InlineData("user with spaces")]
        [InlineData("user@domain.com")]
        [InlineData("123456")]
        public void UserInfo_WithDifferentUserName_SetsUserNameCorrectly(string userName)
        {
            // Act
            var userInfo = new UserInfo { UserName = userName };

            // Assert
            Assert.Equal(userName, userInfo.UserName);
        }

        [Fact]
        public void UserInfo_PropertiesAreSettable()
        {
            // Arrange
            var userInfo = new UserInfo();

            // Act
            userInfo.UserId = 456;
            userInfo.UserName = "newuser";

            // Assert
            Assert.Equal(456, userInfo.UserId);
            Assert.Equal("newuser", userInfo.UserName);
        }

        [Fact]
        public void UserInfo_PropertiesCanBeChanged()
        {
            // Arrange
            var userInfo = new UserInfo
            {
                UserId = 123,
                UserName = "original"
            };

            // Act
            userInfo.UserId = 789;
            userInfo.UserName = "updated";

            // Assert
            Assert.Equal(789, userInfo.UserId);
            Assert.Equal("updated", userInfo.UserName);
        }

        [Fact]
        public void UserInfo_WithNullUserName_SetsUserNameToNull()
        {
            // Act
            var userInfo = new UserInfo { UserName = null };

            // Assert
            Assert.Null(userInfo.UserName);
        }
    }
}
