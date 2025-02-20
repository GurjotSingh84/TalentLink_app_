using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using TalentLink_app.Models;
using TalentLink_app.Services;
namespace TalentLink_app
{
    public partial class RecruiterHomePage : ContentPage
    {
        private readonly FirebaseJobService _jobService = new FirebaseJobService();
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();
        public RecruiterHomePage()
        {
            InitializeComponent();
        }

        private async void GoToPostJob(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PostJobs());
        }

        private async void GoToPostedJobs(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ViewPostedJobs());
        }

        private async void GoToCandidates(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SelectJobForCandidatesPage());
        }


        private async void GoToMessaging(object sender, EventArgs e)
        {
            {
                await Navigation.PushAsync(new MessagingPage());
            }
        }



        private async void GoToNotifications(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NotificationPage());
        }

        private async void GoToProfile(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfileSettingsPage());
        }

        private async void Logout(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}


