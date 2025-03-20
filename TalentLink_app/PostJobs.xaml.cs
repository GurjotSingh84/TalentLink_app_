using System;
using Microsoft.Maui.Controls;
using TalentLink_app.Models;
using TalentLink_app.Services;

namespace TalentLink_app
{
    public partial class PostJobs : ContentPage
    {
        private readonly FirebaseJobService _jobService = new FirebaseJobService();
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();

        public PostJobs()
        {
            InitializeComponent();
        }

        private async void OnPostJobClicked(object sender, EventArgs e)
        {
            string recruiterId = await _authService.GetUserId(); // ✅ Get Recruiter ID
            if (string.IsNullOrEmpty(recruiterId))
            {
                await DisplayAlert("Error", "Authentication required.", "OK");
                return;
            }

            var job = new Job
            {
                JobId = Guid.NewGuid().ToString(),  // ✅ Generate unique Job ID
                RecruiterId = recruiterId,  // ✅ Store Recruiter ID
                JobTitle = JobTitleEntry.Text,
                JobDescription = JobDescriptionEditor.Text,
                PayRate = PayRateEntry.Text,
                Location = LocationEntry.Text,
                PostedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),  // ✅ Store timestamp

                // ✅ Add Company Details
                CompanyName = CompanyNameEntry.Text,
                CompanyWebsite = CompanyWebsiteEntry.Text,

                // ✅ Add Contact Information
                ContactName = ContactNameEntry.Text,
                ContactEmail = ContactEmailEntry.Text,
                ContactPhone = ContactPhoneEntry.Text
            };


            bool success = await _jobService.PostJob(job);

            await DisplayAlert(success ? "Success" : "Error",
                success ? "Job posted successfully!" : "Failed to post job.", "OK");
        }
    }
}

