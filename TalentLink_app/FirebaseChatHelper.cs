using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FirebaseChatHelper
{
    private static readonly string FirebaseURL = "https://aimadeinafrica-9e4ee-default-rtdb.firebaseio.com/";
    private readonly FirebaseClient firebase = new FirebaseClient(FirebaseURL);

    // Send a message to Firebase
    public async Task SendMessage(string senderId, string senderName, string receiverId, string message)
    {
        string chatId = GetChatId(senderId, receiverId);

        var newMessage = new MessageModel
        {
            Id = Guid.NewGuid().ToString(),
            ChatId = chatId,
            SenderId = senderId,
            SenderName = senderName,
            ReceiverId = receiverId,
            Message = message,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        await firebase.Child("messages").Child(chatId).PostAsync(newMessage);
    }

    // Get the list of messages from Firebase
    public async Task<List<MessageModel>> GetMessages(string senderId, string receiverId)
    {
        string chatId = GetChatId(senderId, receiverId);
        var messages = await firebase.Child("messages").Child(chatId).OnceAsync<MessageModel>();

        return messages.Select(m => m.Object)
                       .OrderBy(m => m.Timestamp)
                       .ToList();
    }

    // Delete a message from Firebase by its Id
    public async Task<bool> DeleteMessage(string senderId, string receiverId, string messageId)
    {
        string chatId = GetChatId(senderId, receiverId);

        // Fetch the message from Firebase using messageId
        var messageToDelete = (await firebase.Child("messages").Child(chatId)
                                            .OnceAsync<MessageModel>())
                                            .FirstOrDefault(m => m.Object.Id == messageId);

        if (messageToDelete != null)
        {
            // Delete the message from Firebase
            await firebase.Child("messages").Child(chatId).Child(messageToDelete.Key).DeleteAsync();
            return true;
        }

        return false; // If message not found, return false
    }

    // Generate a unique chatId based on senderId and receiverId
    private string GetChatId(string senderId, string receiverId)
    {
        return senderId.CompareTo(receiverId) < 0 ? $"{senderId}{receiverId}" : $"{receiverId}{senderId}";
    }
}

public class MessageModel
{
    public string Id { get; set; }
    public string ChatId { get; set; }
    public string SenderId { get; set; }
    public string SenderName { get; set; }
    public string ReceiverId { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }
    public bool IsSender { get; set; }
}