using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace TalentLink_app
{
    public partial class MessagingPage : ContentPage
    {
        private readonly FirebaseChatHelper _chatHelper = new FirebaseChatHelper();
        private readonly string senderId = "candidate_123"; // Fetch from Firebase Auth
        private readonly string senderName = "John Doe"; // Fetch from Firebase Auth
        private readonly string receiverId = "recruiter_456"; // Set dynamically

        public ObservableCollection<MessageModel> Messages { get; set; } = new ObservableCollection<MessageModel>();
        public string ReceiverId => receiverId;

        public MessagingPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadMessages();
        }

        private async void LoadMessages()
        {
            var messages = await _chatHelper.GetMessages(senderId, receiverId);
            Messages.Clear();
            foreach (var message in messages)
            {
                message.IsSender = message.SenderId == senderId; // Determine if the message is sent by the current user
                Messages.Add(message);
            }
        }

        private async void SendMessage_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MessageEntry.Text))
            {
                var messageText = MessageEntry.Text;
                var newMessage = new MessageModel
                {
                    SenderId = senderId,
                    SenderName = senderName,
                    ReceiverId = receiverId,
                    Message = messageText,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    IsSender = true
                };

                await _chatHelper.SendMessage(senderId, senderName, receiverId, messageText);
                Messages.Add(newMessage);
                MessageEntry.Text = string.Empty;
            }
        }

        // Delete Message
        private async void OnDeleteMessage_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var message = button?.BindingContext as MessageModel;

            if (message == null) return;

            // Pass the message's Id to DeleteMessage, not the entire message
            bool success = await _chatHelper.DeleteMessage(senderId, receiverId, message.Id);

            if (success)
            {
                // Remove the message from the collection if delete is successful
                Messages.Remove(message);
            }
            else
            {
                await DisplayAlert("Error", "Failed to delete the message", "OK");
            }
        }
    }
}