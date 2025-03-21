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

            // ✅ Populate job details
            JobTitleEntry.Text = job.JobTitle;
            JobDescriptionEditor.Text = job.JobDescription;
            PayRateEntry.Text = job.PayRate;
            LocationEntry.Text = job.Location;

            // ✅ Populate company details
            CompanyNameEntry.Text = job.CompanyName;
            CompanyWebsiteEntry.Text = job.CompanyWebsite;

            // ✅ Populate contact information
            ContactNameEntry.Text = job.ContactName;
            ContactEmailEntry.Text = job.ContactEmail;
            ContactPhoneEntry.Text = job.ContactPhone;
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            // ✅ Update job fields
            _job.JobTitle = JobTitleEntry.Text;
            _job.JobDescription = JobDescriptionEditor.Text;
            _job.PayRate = PayRateEntry.Text;
            _job.Location = LocationEntry.Text;

            // ✅ Update company details
            _job.CompanyName = CompanyNameEntry.Text;
            _job.CompanyWebsite = CompanyWebsiteEntry.Text;

            // ✅ Update contact information
            _job.ContactName = ContactNameEntry.Text;
            _job.ContactEmail = ContactEmailEntry.Text;
            _job.ContactPhone = ContactPhoneEntry.Text;

            // ✅ Call update service
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
