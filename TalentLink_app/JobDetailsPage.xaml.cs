using System;
using Microsoft.Maui.Controls;
using TalentLink_app.Models;

namespace TalentLink_app
{
    public partial class JobDetailsPage : ContentPage
    {
        private Job _selectedJob;

        public JobDetailsPage(Job job)
        {
            InitializeComponent();

            if (job == null)
            {
                DisplayAlert("Error", "Job details not available.", "OK");
                Navigation.PopAsync(); // Navigate back if job data is null
                return;
            }

            _selectedJob = job;
            BindingContext = _selectedJob;
        }

        private async void OnApplyClicked(object sender, EventArgs e)
        {
            if (_selectedJob != null)
            {
                await Navigation.PushAsync(new ApplyPage(_selectedJob.JobId)); // ✅ Pass Job ID to ApplyPage
            }
            else
            {
                await DisplayAlert("Error", "Invalid job selection.", "OK");
            }
        }
    }
}





