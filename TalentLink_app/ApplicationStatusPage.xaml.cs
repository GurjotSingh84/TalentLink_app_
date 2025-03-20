using Microsoft.Maui.Controls;
using System;
using System.Reactive.Linq;
using TalentLink_app.Models;
using TalentLink_app.Services;

namespace TalentLink_app
{
    public partial class ApplicationStatusPage : ContentPage
    {
        private readonly FirebaseJobService _jobService = new FirebaseJobService();
        private IDisposable _statusSubscription;
        private string _applicationId;

        public ApplicationStatusPage() : this("defaultApplicationId") { }

        public ApplicationStatusPage(string applicationId)
        {
            InitializeComponent();
            _applicationId = applicationId;
            SubscribeToApplicationStatus();
        }


        private void SubscribeToApplicationStatus()
        {
            _statusSubscription = _jobService.SubscribeToApplicationStatus(_applicationId)
                .Subscribe(application =>
                {
                    if (application != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            statusLabel.Text = $"Your application status: {application.Status}";
                        });
                    }
                });
        }

        // Refresh button click event handler
        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            try
            {
                var application = await _jobService.GetApplicationById(_applicationId);
                if (application != null)
                {
                    statusLabel.Text = $"Your application status: {application.Status}";
                }
                else
                {
                    statusLabel.Text = "Status not available.";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to refresh status: {ex.Message}", "OK");
            }
        }

        // Back button click event handler
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _statusSubscription?.Dispose(); // Unsubscribe when leaving the page
        }
    }
}
