using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using TalentLink_app.Models;
using TalentLink_app.Services;

namespace TalentLink_app
{
    public partial class AppliedJobsPage : ContentPage
    {
        private readonly FirebaseJobService _jobService = new FirebaseJobService();
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();

        public ObservableCollection<Job> AppliedJobs { get; set; } = new ObservableCollection<Job>();
        private string _candidateId;

        public AppliedJobsPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadAppliedJobs();
        }

        // 🔹 Load Applied Jobs for Logged-in Candidate
        private async void LoadAppliedJobs()
        {
            _candidateId = await _authService.GetUserId(); // ✅ Fetch UID of logged-in candidate

            if (string.IsNullOrEmpty(_candidateId))
            {
                await DisplayAlert("Error", "Authentication required.", "OK");
                return;
            }

            var appliedJobsList = await _jobService.GetAppliedJobsByCandidate(_candidateId); // 🔹 Fetch applied jobs
            AppliedJobs.Clear();

            foreach (var job in appliedJobsList)
            {
                AppliedJobs.Add(job);
            }
        }

        // 🔹 Navigate to Job Details Page
        private async void OnJobSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0) return;

            var selectedJob = e.CurrentSelection[0] as Job;
            if (selectedJob == null) return;

            await Navigation.PushAsync(new ApplicationStatusPage());
            ((CollectionView)sender).SelectedItem = null;
        }

        // 🔹 Withdraw Application
        private async void OnWithdrawApplication(object sender, EventArgs e)
        {
            var button = sender as Button;
            var job = button?.BindingContext as Job;
            if (job == null) return;

            bool confirm = await DisplayAlert("Withdraw Application",
                "Are you sure you want to withdraw your application?", "Yes", "No");

            if (confirm)
            {
                bool success = await _jobService.WithdrawApplication(job.JobId, _candidateId);
                if (success)
                {
                    AppliedJobs.Remove(job);
                    await DisplayAlert("Success", "Application withdrawn successfully!", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to withdraw application.", "OK");
                }
            }
        }
    }
}





