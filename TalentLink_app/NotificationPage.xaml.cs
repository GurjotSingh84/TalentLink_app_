using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Maui.Controls;
using TalentLink_app.Models;
using TalentLink_app.Services;

namespace TalentLink_app
{
    public partial class NotificationPage : ContentPage
    {
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();
        private readonly FirebaseClient _firebase = new FirebaseClient("https://aimadeinafrica-9e4ee-default-rtdb.firebaseio.com/");
        public ObservableCollection<Notification> Notifications { get; set; } = new ObservableCollection<Notification>();

        public NotificationPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadNotifications();
        }

        private async void LoadNotifications()
        {
            try
            {
                string uid = await _authService.GetUserId();
                var notifications = await _firebase.Child("Notifications").Child(uid).OnceAsync<Notification>();

                Notifications.Clear();
                foreach (var notification in notifications.Select(n => n.Object))
                {
                    Notifications.Add(notification);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading notifications: {ex.Message}");
            }
        }
    }
}
