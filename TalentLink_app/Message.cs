namespace TalentLink_app.Models
{
    public class Message
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Text { get; set; }
        public string Timestamp { get; set; }

        public Message() { }

        public Message(string senderId, string receiverId, string text, string timestamp)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            Text = text;
            Timestamp = timestamp;
        }
    }
}
