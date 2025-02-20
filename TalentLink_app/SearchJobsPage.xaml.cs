using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TalentLink_app.Models;
using TalentLink_app.Services;

namespace TalentLink_app
{
    public partial class SearchJobsPage : ContentPage
    {
        private readonly FirebaseJobService _jobService = new FirebaseJobService();
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();
        public ObservableCollection<Job> Jobs { get; set; } = new ObservableCollection<Job>();
        private ObservableCollection<Job> _filteredJobs;
        public ObservableCollection<Job> FilteredJobs
        {
            get { return _filteredJobs; }
            set
            {
                _filteredJobs = value;
                OnPropertyChanged(nameof(FilteredJobs));
            }
        }

        private string _candidateId; // UID as Candidate ID

        public SearchJobsPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadCandidateId();
            LoadJobs();
        }

        private async void LoadCandidateId()
        {
            _candidateId = await _authService.GetUserId(); // ✅ Get UID
        }

        private async void LoadJobs()
        {
            var jobsFromDb = await _jobService.GetAllJobs();
            Jobs.Clear();
            foreach (var job in jobsFromDb)
            {
                Jobs.Add(job);
            }
            FilterJobs(JobSearchBar.Text);
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            FilterJobs(e.NewTextValue);
        }

        private void FilterJobs(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                FilteredJobs = new ObservableCollection<Job>(Jobs);
            }
            else
            {
                var filteredList = Jobs.Where(j =>
                                    j.JobTitle.ToLower().Contains(searchText.ToLower()) ||
                                    j.JobId.ToLower().Contains(searchText.ToLower()) ||
                                    j.Location.ToLower().Contains(searchText.ToLower()))
                                    .ToList();

                FilteredJobs = new ObservableCollection<Job>(filteredList);
            }
        }

        private async void OnJobSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Job selectedJob)
            {
                await Navigation.PushAsync(new JobDetailsPage(selectedJob));
            }
        }

        private async void OnSaveJobClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var jobId = button?.CommandParameter as string;

            if (string.IsNullOrEmpty(jobId)) return;

            bool success = await _jobService.SaveJob(_candidateId, jobId);
            await DisplayAlert(success ? "Success" : "Error",
                success ? "Job saved successfully!" : "Failed to save job.", "OK");
        }
    }
}




