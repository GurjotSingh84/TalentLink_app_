using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using TalentLink_app.Models;
using TalentLink_app.Services;

namespace TalentLink_app
{
    public partial class ViewPostedJobs : ContentPage
    {
        private readonly FirebaseJobService _jobService = new FirebaseJobService();
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();
        public ObservableCollection<Job> Jobs { get; set; } = new ObservableCollection<Job>();
        private string _recruiterId;

        public ViewPostedJobs()
        {
            InitializeComponent();
            BindingContext = this;
            LoadJobs();
        }

        private async void LoadJobs()
        {
            _recruiterId = await _authService.GetUserId(); // ✅ Get UID as Recruiter ID
            if (string.IsNullOrEmpty(_recruiterId))
            {
                await DisplayAlert("Error", "Authentication required.", "OK");
                return;
            }

            var jobsList = await _jobService.GetJobsByRecruiter(_recruiterId); // 🔹 Fetch only jobs posted by recruiter
            Jobs.Clear();
            foreach (var job in jobsList)
            {
                Jobs.Add(job);
            }
        }

        private async void OnJobSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0) return;

            var selectedJob = e.CurrentSelection[0] as Job;
            if (selectedJob == null) return;

            await Navigation.PushAsync(new EditJobPage(selectedJob));
            ((CollectionView)sender).SelectedItem = null;
        }

        private async void OnEditJob(object sender, EventArgs e)
        {
            var button = sender as Button;
            var job = button?.BindingContext as Job;
            if (job == null) return;

            await Navigation.PushAsync(new EditJobPage(job));
        }

        private async void OnDeleteJob(object sender, EventArgs e)
        {
            var button = sender as Button;
            var job = button?.BindingContext as Job;
            if (job == null) return;

            bool confirm = await DisplayAlert("Delete Job", "Are you sure you want to delete this job?", "Yes", "No");
            if (confirm)
            {
                bool success = await _jobService.DeleteJob(job.JobId);
                if (success)
                {
                    Jobs.Remove(job);
                    await DisplayAlert("Success", "Job deleted successfully!", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to delete job.", "OK");
                }
            }
        }
    }
}


