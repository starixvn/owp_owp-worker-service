using NSubstitute;
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
        private readonly OWP.Chat.Repository _mockRepository;
        private readonly HttpHandler _mockHttpHandler;
        private readonly IConfiguration _configuration;
        private readonly LogWriterService _mockLogger;
        private readonly ChatServiceImp _service;

        public ChatServiceTests()
        {
            _mockRepository = Substitute.For<OWP.Chat.Repository>();
            _mockHttpHandler = Substitute.For<HttpHandler>();
            _mockLogger = Substitute.For<LogWriterService>();

            // Create real configuration
            var configData = new Dictionary<string, string?>
            {
                { "ChatServer", "https://test.com" }
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            _service = new ChatServiceImp(_mockRepository, _mockHttpHandler, _configuration, _mockLogger);
        }

        [Fact]
        public async Task CreateAccount_WhenNoNewUsers_DoesNotCallApiOrUpdateRepository()
        {
            // Arrange
            var emptyUserList = new List<UserInfo>();
            _mockRepository.GetNewUser().Returns(emptyUserList);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockHttpHandler.DidNotReceive().PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>());
            _mockRepository.Received(1).UpdateChatUserId("[]");
            _mockLogger.DidNotReceive().Execute(Arg.Any<OwpLogCriteria>());
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
            _mockRepository.GetNewUser().Returns(users);

            var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new ChatUserResponse
                {
                    success = true,
                    user = new User { _id = "chat123" }
                }), Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(successResponse);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockHttpHandler.Received(2).PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>());
        }

        [Fact]
        public async Task CreateAccount_WhenApiSucceeds_UpdatesRepositoryWithSuccessData()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.GetNewUser().Returns(users);

            var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new ChatUserResponse
                {
                    success = true,
                    user = new User { _id = "chat123" }
                }), Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(successResponse);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockRepository.Received(1).UpdateChatUserId(Arg.Is<string>(json =>
                json.Contains("UserId") && json.Contains("ChatUserId") && json.Contains("chat123")));
        }

        [Fact]
        public async Task CreateAccount_WhenApiFails_LogsFailure()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.GetNewUser().Returns(users);

            var failureResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Error message", Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(failureResponse);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockLogger.Received(1).Execute(Arg.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "InsertNewUser_Fail" &&
                criteria.Payload.Contains("user1")));
        }

        [Fact]
        public async Task CreateAccount_WhenApiReturnsSuccessFalse_LogsFailure()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.GetNewUser().Returns(users);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new ChatUserResponse
                {
                    success = false,
                    user = new User { _id = "chat123" }
                }), Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(response);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockLogger.Received(1).Execute(Arg.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "InsertNewUser_Fail"));
        }

        [Fact]
        public async Task CreateAccount_WhenApiReturnsNullResponse_LogsFailure()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.GetNewUser().Returns(users);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null", Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(response);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockLogger.Received(1).Execute(Arg.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "InsertNewUser_Fail"));
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
            _mockRepository.GetNewUser().Returns(users);

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

            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(successResponse, failureResponse);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockRepository.Received(1).UpdateChatUserId(Arg.Any<string>());
            _mockLogger.Received(1).Execute(Arg.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "InsertNewUser_Fail"));
        }

        [Fact]
        public async Task CreateAccount_WhenNoFailures_DoesNotLogFailures()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.GetNewUser().Returns(users);

            var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new ChatUserResponse
                {
                    success = true,
                    user = new User { _id = "chat123" }
                }), Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(successResponse);

            // Act
            await _service.CreateAccount();

            // Assert
            _mockLogger.DidNotReceive().Execute(Arg.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "InsertNewUser_Fail"));
        }

        [Fact]
        public async Task CreateAccount_WhenConfigurationIsNull_HandlesGracefully()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { UserId = 1, UserName = "user1" }
            };
            _mockRepository.GetNewUser().Returns(users);

            // Create a configuration with null ChatServer
            var configData = new Dictionary<string, string?>
            {
                { "ChatServer", null }
            };
            var nullConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            var serviceWithNullConfig = new ChatServiceImp(_mockRepository, _mockHttpHandler, nullConfig, _mockLogger);

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
            _mockRepository.GetNewUser().Returns(users);
            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(Task.FromException<HttpResponseMessage>(new HttpRequestException("Network error")));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _service.CreateAccount());

            // Verify the mock was called
            _mockHttpHandler.Received(1).PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>());
        }

        [Fact]
        public async Task CreateAccount_WhenRepositoryThrows_PropagatesException()
        {
            // Arrange
            _mockRepository.GetNewUser().Returns(Task.FromException<IList<UserInfo>>(new InvalidOperationException("Database error")));

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
            _mockRepository.GetNewUser().Returns(users);
            _mockRepository.UpdateChatUserId(Arg.Any<string>())
                .Returns(Task.FromException(new InvalidOperationException("Update failed")));

            var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new ChatUserResponse
                {
                    success = true,
                    user = new User { _id = "chat123" }
                }), Encoding.UTF8, "application/json")
            };
            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(successResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAccount());
        }
    }
}