using Moq;
using OWP.Chat.Service;
using OWP.Chat.Models;
using OWP.Services;
using Microsoft.Extensions.Configuration;
using Olympus.OWP.Log.Data.Repository.Services;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using OWP.Chat;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Microsoft.Extensions.Configuration.Memory;

namespace OWP.Chat.UnitTests.Services
{
    public class ChatServiceTests
    {
        private readonly Mock<OWP.Chat.Repository> _mockRepository;
        private readonly Mock<HttpHandler> _mockHttpHandler;
        private readonly IConfiguration _configuration;
        private readonly Mock<LogWriterService> _mockLogger;
        private readonly ChatServiceImp _service;

        public ChatServiceTests()
        {
            _mockRepository = new Mock<OWP.Chat.Repository>();
            _mockHttpHandler = new Mock<HttpHandler>();
            _mockLogger = new Mock<LogWriterService>();

            // Create real configuration
            var configData = new Dictionary<string, string?>
            {
                { "ChatServer", "https://test.com" }
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            _service = new ChatServiceImp(_mockRepository.Object, _mockHttpHandler.Object, _configuration, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAccount_WhenNoNewUsers_DoesNotCallApiOrUpdateRepository()
        {
            // Arrange
            var emptyUserList = new List<UserInfo>();
            _mockRepository.Setup(x => x.GetNewUser()).ReturnsAsync(emptyUserList);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Never);
            _mockRepository.Verify(x => x.UpdateChatUserId("[]"), Times.Once);
            _mockLogger.Verify(x => x.Execute(It.IsAny<OwpLogCriteria>()), Times.Never);
        }

        [Fact]
        public async Task CreateAccount_WhenUsersExist_CallsApiForEachUser()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" },
                new UserInfo { UserId = 2, UserName = "user2" }
            };
            _mockRepository.Setup(x => x.GetNewUser()).ReturnsAsync(users);

            var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new ChatUserResponse
                {
                    success = true,
                    user = new User { _id = "chat123" }
                }), Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(successResponse);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Exactly(2));
        }

        [Fact]
        public async Task CreateAccount_WhenApiSucceeds_UpdatesRepositoryWithSuccessData()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.Setup(x => x.GetNewUser()).ReturnsAsync(users);

            var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new ChatUserResponse
                {
                    success = true,
                    user = new User { _id = "chat123" }
                }), Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(successResponse);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockRepository.Verify(x => x.UpdateChatUserId(It.Is<string>(json =>
                json.Contains("UserId") && json.Contains("ChatUserId") && json.Contains("chat123"))), Times.Once);
        }

        [Fact]
        public async Task CreateAccount_WhenApiFails_LogsFailure()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.Setup(x => x.GetNewUser()).ReturnsAsync(users);

            var failureResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Error message", Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(failureResponse);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockLogger.Verify(x => x.Execute(It.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "InsertNewUser_Fail" &&
                criteria.Payload.Contains("user1"))), Times.Once);
        }

        [Fact]
        public async Task CreateAccount_WhenApiReturnsSuccessFalse_LogsFailure()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.Setup(x => x.GetNewUser()).ReturnsAsync(users);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new ChatUserResponse
                {
                    success = false,
                    user = new User { _id = "chat123" }
                }), Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(response);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockLogger.Verify(x => x.Execute(It.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "InsertNewUser_Fail")), Times.Once);
        }

        [Fact]
        public async Task CreateAccount_WhenApiReturnsNullResponse_LogsFailure()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.Setup(x => x.GetNewUser()).ReturnsAsync(users);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null", Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(response);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockLogger.Verify(x => x.Execute(It.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "InsertNewUser_Fail")), Times.Once);
        }

        [Fact]
        public async Task CreateAccount_WhenMixedResults_HandlesBothSuccessAndFailure()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" },
                new UserInfo { UserId = 2, UserName = "user2" }
            };
            _mockRepository.Setup(x => x.GetNewUser()).ReturnsAsync(users);

            var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new ChatUserResponse
                {
                    success = true,
                    user = new User { _id = "chat123" }
                }), Encoding.UTF8, "application/json")
            };

            var failureResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Error", Encoding.UTF8, "application/json")
            };

            _mockHttpHandler.SetupSequence(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(successResponse)
                .ReturnsAsync(failureResponse);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockRepository.Verify(x => x.UpdateChatUserId(It.IsAny<string>()), Times.Once);
            _mockLogger.Verify(x => x.Execute(It.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "InsertNewUser_Fail")), Times.Once);
        }

        [Fact]
        public async Task CreateAccount_WhenNoFailures_DoesNotLogFailures()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.Setup(x => x.GetNewUser()).ReturnsAsync(users);

            var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new ChatUserResponse
                {
                    success = true,
                    user = new User { _id = "chat123" }
                }), Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(successResponse);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockLogger.Verify(x => x.Execute(It.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "InsertNewUser_Fail")), Times.Never);
        }

        [Fact]
        public async Task CreateAccount_WhenConfigurationIsNull_HandlesGracefully()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.Setup(x => x.GetNewUser()).ReturnsAsync(users);

            // Create a configuration with null ChatServer
            var configData = new Dictionary<string, string?>
            {
                { "ChatServer", null }
            };
            var nullConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            var serviceWithNullConfig = new ChatServiceImp(_mockRepository.Object, _mockHttpHandler.Object, nullConfig, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => serviceWithNullConfig.CreateAccount());
        }

        [Fact]
        public async Task CreateAccount_WhenHttpHandlerThrows_LogsFailure()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.Setup(x => x.GetNewUser()).ReturnsAsync(users);
            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _service.CreateAccount());

            // Verify the mock was called
            _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Once);
        }

        [Fact]
        public async Task CreateAccount_WhenRepositoryThrows_PropagatesException()
        {
            // Arrange
            _mockRepository.Setup(x => x.GetNewUser()).ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAccount());
        }

        [Fact]
        public async Task CreateAccount_WhenUpdateRepositoryThrows_PropagatesException()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.Setup(x => x.GetNewUser()).ReturnsAsync(users);
            _mockRepository.Setup(x => x.UpdateChatUserId(It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Update failed"));

            var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new ChatUserResponse
                {
                    success = true,
                    user = new User { _id = "chat123" }
                }), Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(successResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAccount());
        }
    }
}
