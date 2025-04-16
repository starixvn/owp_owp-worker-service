using OWP.Services;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Olympus.OWP.Log.Data.Repository.Services;
using OWP.Chat.Models;

namespace OWP.Chat.Service
{
    public interface ChatService
    {
        Task CreateAccount();
    }

    public class ChatServiceImp(
        Repository repository,
        HttpHandler httpHandler,
        IConfiguration configuration,
        LogWriterService logger) : ChatService
    {
        private const int DELAY = 3000;

        public async Task CreateAccount()
        {
            var newUser = await repository.GetNewUser();
            var url = configuration.GetValue<string>("ChatServer");
            var userCreatedSuccess = new List<object>();
            var userCreatedFail = new List<object>();
            foreach (var user in newUser)
            {
                var response= await CreateAccount(user, url);
                await HandleResponse(response, userCreatedSuccess, userCreatedFail, user);
                await Task.Delay(DELAY);
            }
            await repository.UpdateChatUserId(System.Text.Json.JsonSerializer.Serialize(userCreatedSuccess));
            await WriteLog(userCreatedFail, "InsertNewUser_Fail");
        }

        private async Task HandleResponse(
            HttpResponseMessage response,
            IList<object> userCreatedSuccess,
            IList<object> userCreatedFail,
            UserInfo user
            )
        {
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var chatUserInfo = JsonConvert.DeserializeObject<ChatUserResponse>(content);
                if (chatUserInfo is not null && chatUserInfo.success)
                {
                    userCreatedSuccess.Add(new
                    {
                        UserId = user.UserId,
                        ChatUserId = chatUserInfo.user._id,
                    });
                    return;
                }
            }
            userCreatedFail.Add(new
            {
                UserId = user.UserId,
                UserName = user.UserName,
                success = false,
                message= content
            });
        }

        private async Task<HttpResponseMessage> CreateAccount(UserInfo user, string chatServerUrl)
        {
            var json = new
            {
                username = user.UserName.Trim(),
                email = $"{user.UserName.Trim()}@gmail.com",
                pass = $"password",
                name = $"{user.UserName.Trim()}"
            };
            var jsonString = System.Text.Json.JsonSerializer.Serialize(json);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            return await httpHandler.PostAsync($"{chatServerUrl}/api/v1/users.register", content);
        }

        private async Task WriteLog(IList<object> newUser, string action)
        {
            await logger.Execute(new OwpLogCriteria
            {
                CreatedDate = DateTime.Now.AddHours(12),
                Action = action,
                Payload = JsonConvert.SerializeObject(newUser)
            });
        }
    }
}
