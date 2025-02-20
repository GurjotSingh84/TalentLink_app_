using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace TalentLink_app
{
    public partial class NotificationPage : ContentPage
    {
        public ObservableCollection<string> Notifications { get; set; } = new ObservableCollection<string>();

        public NotificationPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            Notifications.Add("New job posted: Software Engineer");
            Notifications.Add("Your application for UX Designer was viewed");
        }
    }
}