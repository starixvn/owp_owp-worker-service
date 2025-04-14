using OWP.Services;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Olympus.OWP.Log.Data.Repository.Services;

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
        public async Task CreateAccount()
        {
            var createResult= new List<object>();
          
            var newUser=await repository.GetNewUser();
            var url = configuration.GetValue<string>("ChatServer");

            foreach (var user in newUser)
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

                var response = await httpHandler.PostAsync($"{url}/api/v1/users.register", content);

                createResult.Add(new
                {
                    UserName= user.UserName,
                    UserId= user.UserId,
                    IsSuccess= response.IsSuccessStatusCode
                });
               
                await Task.Delay(3000);
            }

            await WriteLog(createResult, "InsertNewUser");

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
