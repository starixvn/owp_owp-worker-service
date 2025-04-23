using Fanex.Data.Repository;

namespace OWP.Chat.Criteria
{
    public class ChatUserInfoCriteria : INonQueryCommand
    {
        public string UserJson { get; set; }

        public string GetSettingKey() => "OWP_UpdateChatUserId";


        public bool IsValid() => true;
        
    }
}
