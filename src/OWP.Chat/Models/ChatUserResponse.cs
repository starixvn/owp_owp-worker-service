namespace OWP.Chat.Models
{
    using System;
    using System.Collections.Generic;

    public class ChatUserResponse
    {
        public User user { get; set; }
        public bool success { get; set; }
    }

    public class User
    {
        public string _id { get; set; }
        public DateTime createdAt { get; set; }
        public Services services { get; set; }
        public List<Email> emails { get; set; }
        public string type { get; set; }
        public List<string> roles { get; set; }
        public string status { get; set; }
        public bool active { get; set; }
        public DateTime _updatedAt { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public List<string> __rooms { get; set; }
    }

    public class Services
    {
        public Password password { get; set; }
        public Email2fa email2fa { get; set; }
        public EmailService email { get; set; }
    }

    public class Password
    {
        public string bcrypt { get; set; }
        public Enroll enroll { get; set; }
    }

    public class Enroll
    {
        public string token { get; set; }
        public string email { get; set; }
        public DateTime when { get; set; }
        public string reason { get; set; }
    }

    public class Email2fa
    {
        public bool enabled { get; set; }
        public DateTime changedAt { get; set; }
    }

    public class EmailService
    {
        public List<VerificationToken> verificationTokens { get; set; }
    }

    public class VerificationToken
    {
        public string token { get; set; }
        public string address { get; set; }
        public DateTime when { get; set; }
    }

    public class Email
    {
        public string address { get; set; }
        public bool verified { get; set; }
    }

}
