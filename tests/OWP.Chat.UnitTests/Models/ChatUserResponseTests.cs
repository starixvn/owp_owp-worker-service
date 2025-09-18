using OWP.Chat.Models;
using Newtonsoft.Json;

namespace OWP.Chat.UnitTests.Models
{
    public class ChatUserResponseTests
    {
        [Fact]
        public void ChatUserResponse_DefaultConstructor_InitializesProperties()
        {
            // Act
            var response = new ChatUserResponse();

            // Assert
            Assert.Null(response.user);
            Assert.False(response.success);
        }

        [Fact]
        public void ChatUserResponse_WithValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            var user = new User
            {
                _id = "chat123",
                name = "Test User",
                username = "testuser"
            };

            // Act
            var response = new ChatUserResponse
            {
                user = user,
                success = true
            };

            // Assert
            Assert.Equal(user, response.user);
            Assert.True(response.success);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ChatUserResponse_WithDifferentSuccessValues_SetsSuccessCorrectly(bool success)
        {
            // Act
            var response = new ChatUserResponse { success = success };

            // Assert
            Assert.Equal(success, response.success);
        }

        [Fact]
        public void ChatUserResponse_WithNullUser_SetsUserToNull()
        {
            // Act
            var response = new ChatUserResponse { user = null };

            // Assert
            Assert.Null(response.user);
        }

        [Fact]
        public void ChatUserResponse_JsonSerialization_WorksCorrectly()
        {
            // Arrange
            var response = new ChatUserResponse
            {
                success = true,
                user = new User
                {
                    _id = "chat123",
                    name = "Test User",
                    username = "testuser",
                    createdAt = new DateTime(2024, 1, 1),
                    active = true,
                    status = "online"
                }
            };

            // Act
            var json = JsonConvert.SerializeObject(response);
            var deserializedResponse = JsonConvert.DeserializeObject<ChatUserResponse>(json);

            // Assert
            Assert.NotNull(json);
            Assert.NotNull(deserializedResponse);
            Assert.Equal(response.success, deserializedResponse.success);
            Assert.Equal(response.user._id, deserializedResponse.user._id);
            Assert.Equal(response.user.name, deserializedResponse.user.name);
            Assert.Equal(response.user.username, deserializedResponse.user.username);
        }

        [Fact]
        public void User_DefaultConstructor_InitializesProperties()
        {
            // Act
            var user = new User();

            // Assert
            Assert.Null(user._id);
            Assert.Equal(default(DateTime), user.createdAt);
            Assert.Null(user.services);
            Assert.Null(user.emails);
            Assert.Null(user.type);
            Assert.Null(user.roles);
            Assert.Null(user.status);
            Assert.False(user.active);
            Assert.Equal(default(DateTime), user._updatedAt);
            Assert.Null(user.name);
            Assert.Null(user.username);
            Assert.Null(user.__rooms);
        }

        [Fact]
        public void User_WithValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            var createdAt = new DateTime(2024, 1, 1);
            var updatedAt = new DateTime(2024, 1, 2);
            var services = new OWP.Chat.Models.Services();
            var emails = new List<Email>();
            var roles = new List<string> { "user" };
            var rooms = new List<string> { "room1", "room2" };

            // Act
            var user = new User
            {
                _id = "chat123",
                createdAt = createdAt,
                services = services,
                emails = emails,
                type = "user",
                roles = roles,
                status = "online",
                active = true,
                _updatedAt = updatedAt,
                name = "Test User",
                username = "testuser",
                __rooms = rooms
            };

            // Assert
            Assert.Equal("chat123", user._id);
            Assert.Equal(createdAt, user.createdAt);
            Assert.Equal(services, user.services);
            Assert.Equal(emails, user.emails);
            Assert.Equal("user", user.type);
            Assert.Equal(roles, user.roles);
            Assert.Equal("online", user.status);
            Assert.True(user.active);
            Assert.Equal(updatedAt, user._updatedAt);
            Assert.Equal("Test User", user.name);
            Assert.Equal("testuser", user.username);
            Assert.Equal(rooms, user.__rooms);
        }

        [Fact]
        public void Services_DefaultConstructor_InitializesProperties()
        {
            // Act
            var services = new OWP.Chat.Models.Services();

            // Assert
            Assert.Null(services.password);
            Assert.Null(services.email2fa);
            Assert.Null(services.email);
        }

        [Fact]
        public void Services_WithValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            var password = new Password();
            var email2fa = new Email2fa();
            var email = new OWP.Chat.Models.EmailService();

            // Act
            var services = new OWP.Chat.Models.Services
            {
                password = password,
                email2fa = email2fa,
                email = email
            };

            // Assert
            Assert.Equal(password, services.password);
            Assert.Equal(email2fa, services.email2fa);
            Assert.Equal(email, services.email);
        }

        [Fact]
        public void Password_DefaultConstructor_InitializesProperties()
        {
            // Act
            var password = new Password();

            // Assert
            Assert.Null(password.bcrypt);
            Assert.Null(password.enroll);
        }

        [Fact]
        public void Password_WithValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            var enroll = new Enroll();

            // Act
            var password = new Password
            {
                bcrypt = "hashedpassword",
                enroll = enroll
            };

            // Assert
            Assert.Equal("hashedpassword", password.bcrypt);
            Assert.Equal(enroll, password.enroll);
        }

        [Fact]
        public void Enroll_DefaultConstructor_InitializesProperties()
        {
            // Act
            var enroll = new Enroll();

            // Assert
            Assert.Null(enroll.token);
            Assert.Null(enroll.email);
            Assert.Equal(default(DateTime), enroll.when);
            Assert.Null(enroll.reason);
        }

        [Fact]
        public void Enroll_WithValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            var when = new DateTime(2024, 1, 1);

            // Act
            var enroll = new Enroll
            {
                token = "enrolltoken",
                email = "test@example.com",
                when = when,
                reason = "registration"
            };

            // Assert
            Assert.Equal("enrolltoken", enroll.token);
            Assert.Equal("test@example.com", enroll.email);
            Assert.Equal(when, enroll.when);
            Assert.Equal("registration", enroll.reason);
        }

        [Fact]
        public void Email2fa_DefaultConstructor_InitializesProperties()
        {
            // Act
            var email2fa = new Email2fa();

            // Assert
            Assert.False(email2fa.enabled);
            Assert.Equal(default(DateTime), email2fa.changedAt);
        }

        [Fact]
        public void Email2fa_WithValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            var changedAt = new DateTime(2024, 1, 1);

            // Act
            var email2fa = new Email2fa
            {
                enabled = true,
                changedAt = changedAt
            };

            // Assert
            Assert.True(email2fa.enabled);
            Assert.Equal(changedAt, email2fa.changedAt);
        }

        [Fact]
        public void EmailService_DefaultConstructor_InitializesProperties()
        {
            // Act
            var emailService = new EmailService();

            // Assert
            Assert.Null(emailService.verificationTokens);
        }

        [Fact]
        public void EmailService_WithValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            var tokens = new List<VerificationToken>();

            // Act
            var emailService = new EmailService
            {
                verificationTokens = tokens
            };

            // Assert
            Assert.Equal(tokens, emailService.verificationTokens);
        }

        [Fact]
        public void VerificationToken_DefaultConstructor_InitializesProperties()
        {
            // Act
            var token = new VerificationToken();

            // Assert
            Assert.Null(token.token);
            Assert.Null(token.address);
            Assert.Equal(default(DateTime), token.when);
        }

        [Fact]
        public void VerificationToken_WithValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            var when = new DateTime(2024, 1, 1);

            // Act
            var token = new VerificationToken
            {
                token = "verificationtoken",
                address = "test@example.com",
                when = when
            };

            // Assert
            Assert.Equal("verificationtoken", token.token);
            Assert.Equal("test@example.com", token.address);
            Assert.Equal(when, token.when);
        }

        [Fact]
        public void Email_DefaultConstructor_InitializesProperties()
        {
            // Act
            var email = new Email();

            // Assert
            Assert.Null(email.address);
            Assert.False(email.verified);
        }

        [Fact]
        public void Email_WithValidData_SetsPropertiesCorrectly()
        {
            // Act
            var email = new Email
            {
                address = "test@example.com",
                verified = true
            };

            // Assert
            Assert.Equal("test@example.com", email.address);
            Assert.True(email.verified);
        }
    }
}
