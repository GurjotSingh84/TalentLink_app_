using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using TalentLink_app.Models;
using TalentLink_app.Services;

namespace TalentLink_app
{
    public partial class ViewCandidatesPage : ContentPage
    {
        private readonly FirebaseJobService _jobService = new FirebaseJobService();
        public ObservableCollection<JobApplication> Applications { get; set; } = new ObservableCollection<JobApplication>();
        private string _jobId;

        public ViewCandidatesPage(Job selectedJob)
        {
            InitializeComponent();
            BindingContext = this;

            if (selectedJob != null)
            {
                _jobId = selectedJob.JobId;
                Title = $"Applicants for {selectedJob.JobTitle}";
                LoadApplicationsForJob();
            }
        }

        private async void LoadApplicationsForJob()
        {
            try
            {
                Console.WriteLine($"🔍 Fetching applications for Job ID: {_jobId}");

                var applicationsList = await _jobService.GetApplicationsForJob(_jobId);

                Console.WriteLine($"✅ Applications Fetched: {applicationsList.Count}");

                Applications.Clear();
                foreach (var application in applicationsList)
                {
                    Console.WriteLine($"🆕 Application: {application.Name} - {application.Email}");
                    Applications.Add(application);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load applications: {ex.Message}", "OK");
            }
        }

        private async void OnResumeTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is JobApplication application)
            {
                if (!string.IsNullOrEmpty(application.ResumeUrl))
                {
                    await Launcher.OpenAsync(new Uri(application.ResumeUrl));
                }
                else
                {
                    await DisplayAlert("Error", "Resume URL is not available.", "OK");
                }
            }
        }

        private async void OnApproveClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string applicationId)
            {
                bool confirm = await DisplayAlert("Approve", "Are you sure you want to approve this candidate?", "Yes", "No");
                if (confirm)
                {
                    try
                    {
                        await _jobService.UpdateApplicationStatus(applicationId, "Approved");
                        await DisplayAlert("Success", "Candidate approved successfully!", "OK");
                        LoadApplicationsForJob(); // Refresh the list
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Failed to approve candidate: {ex.Message}", "OK");
                    }
                }
            }
        }

        private async void OnRejectClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string applicationId)
            {
                bool confirm = await DisplayAlert("Reject", "Are you sure you want to reject this candidate?", "Yes", "No");
                if (confirm)
                {
                    try
                    {
                        await _jobService.UpdateApplicationStatus(applicationId, "Rejected");
                        await DisplayAlert("Success", "Candidate rejected successfully!", "OK");
                        LoadApplicationsForJob(); // Refresh the list
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Failed to reject candidate: {ex.Message}", "OK");
                    }
                }
            }
        }







    }
}





