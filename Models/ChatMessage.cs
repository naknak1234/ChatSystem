using System;

namespace ChatSystem.Models
{
    public class ChatMessage
    {
        public string ChatId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public string Sender { get; set; }
        public string AIModel { get; set; }
        public string Title { get; set; }
    }
}