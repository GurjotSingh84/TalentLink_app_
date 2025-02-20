using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using TalentLink_app.Models;
using TalentLink_app.Services;

namespace TalentLink_app
{
    public partial class SavedJobsPage : ContentPage
    {
        private readonly FirebaseJobService _jobService = new FirebaseJobService();
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();

        public ObservableCollection<Job> SavedJobs { get; set; } = new ObservableCollection<Job>();

        private string _candidateId;

        public SavedJobsPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadCandidateId();
        }

        private async void LoadCandidateId()
        {
            _candidateId = await _authService.GetUserId();
            if (!string.IsNullOrEmpty(_candidateId))
            {
                LoadSavedJobs();
            }
            else
            {
                await DisplayAlert("Error", "Unable to retrieve candidate information. Please try again.", "OK");
                await Navigation.PopAsync();
            }
        }

       

        private async void LoadSavedJobs()
        {
            Console.WriteLine($"🔍 Logged-in Candidate ID: {_candidateId}");
            try
            {
                Console.WriteLine($"🔍 Fetching saved jobs for Candidate ID: {_candidateId}");

                var savedJobsList = await _jobService.GetSavedJobs(_candidateId);

                Console.WriteLine($"✅ Saved Jobs Fetched: {savedJobsList.Count}");

                SavedJobs.Clear();
                foreach (var job in savedJobsList)
                {
                    Console.WriteLine($"🆕 Saved Job: {job.JobTitle} - {job.Location}");
                    SavedJobs.Add(job);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load saved jobs: {ex.Message}", "OK");
            }
        }

        private async void OnUnsaveJobClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Job job)
            {
                bool confirm = await DisplayAlert("Unsave Job", "Are you sure you want to remove this job?", "Yes", "No");
                if (!confirm) return;

                try
                {
                    bool success = await _jobService.UnsaveJob(_candidateId, job.JobId);
                    if (success)
                    {
                        SavedJobs.Remove(job);
                        await DisplayAlert("Success", "Job removed from saved jobs.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to remove job.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                }
            }
        }
        private async void OnJobTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Job job)
            {
                await Navigation.PushAsync(new JobDetailsPage(job));
            }
        }
    }
}

