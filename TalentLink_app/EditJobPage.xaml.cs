using Microsoft.Maui.Controls;
using System;
using TalentLink_app.Models;
using TalentLink_app.Services;

namespace TalentLink_app
{
    public partial class EditJobPage : ContentPage
    {
        private readonly FirebaseJobService _jobService = new FirebaseJobService();
        private Job _job;

        public EditJobPage(Job job)
        {
            InitializeComponent();
            _job = job;
            JobTitleEntry.Text = job.JobTitle;
            JobDescriptionEditor.Text = job.JobDescription;
            PayRateEntry.Text = job.PayRate;
            LocationEntry.Text = job.Location;
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            _job.JobTitle = JobTitleEntry.Text;
            _job.JobDescription = JobDescriptionEditor.Text;
            _job.PayRate = PayRateEntry.Text;
            _job.Location = LocationEntry.Text;

            bool success = await _jobService.EditJob(_job);

            await DisplayAlert(success ? "Success" : "Error",
                success ? "Job updated successfully!" : "Failed to update job.", "OK");

            if (success)
            {
                await Navigation.PopAsync();
            }
        }
    }
}
