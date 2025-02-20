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
        public ObservableCollection<Candidate> Candidates { get; set; } = new ObservableCollection<Candidate>();
        private string _jobId;

        // ✅ Constructor that accepts a Job
        public ViewCandidatesPage(Job selectedJob)
        {
            InitializeComponent();
            BindingContext = this;

            if (selectedJob != null)
            {
                _jobId = selectedJob.JobId;
                Title = $"Candidates for {selectedJob.JobTitle}";
                LoadCandidatesForJob();
            }
        }

        private async void LoadCandidatesForJob()
        {
            try
            {
                Console.WriteLine($"🔍 Fetching candidates for Job ID: {_jobId}");

                var candidatesList = await _jobService.GetCandidatesForJob(_jobId);

                Console.WriteLine($"✅ Candidates Fetched: {candidatesList.Count}");

                Candidates.Clear();
                foreach (var candidate in candidatesList)
                {
                    Console.WriteLine($"🆕 Candidate: {candidate.Name} - {candidate.Email}");
                    Candidates.Add(candidate);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load candidates: {ex.Message}", "OK");
            }
        }

        private async void OnCandidateSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0) return;

            var selectedCandidate = e.CurrentSelection[0] as Candidate;
            if (selectedCandidate == null) return;

            await Navigation.PushAsync(new CandidateDetailsPage(selectedCandidate));
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}





