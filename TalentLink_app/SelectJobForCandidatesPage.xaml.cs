using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using TalentLink_app.Models;
using TalentLink_app.Services;

namespace TalentLink_app
{
    public partial class SelectJobForCandidatesPage : ContentPage
    {
        private readonly FirebaseJobService _jobService = new FirebaseJobService();
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();
        public ObservableCollection<Job> Jobs { get; set; } = new ObservableCollection<Job>();
        private string _recruiterId;

        public SelectJobForCandidatesPage()
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

            // ✅ Fetch only jobs posted by the recruiter
            var jobsList = await _jobService.GetJobsByRecruiter(_recruiterId);
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

            // ✅ Pass the selected job to ViewCandidatesPage
            await Navigation.PushAsync(new ViewCandidatesPage(selectedJob));
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}

