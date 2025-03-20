using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Maui.Controls;
using TalentLink_app.Models;

namespace TalentLink_app
{
    public partial class SearchCandidatesByProfilePage : ContentPage
    {
        private FirebaseClient firebaseClient = new FirebaseClient("https://aimadeinafrica-9e4ee-default-rtdb.firebaseio.com/");
        private List<Candidate> allCandidates = new List<Candidate>();

        public SearchCandidatesByProfilePage()
        {
            InitializeComponent();
            LoadCandidates();
        }

        private async void LoadCandidates()
        {
            try
            {
                var candidates = await firebaseClient.Child("Candidates").OnceAsync<Candidate>();

                allCandidates = candidates.Select(item => new Candidate
                {
                    Name = item.Object.Name,
                    Email = item.Object.Email,
                    Phone = item.Object.Phone,
                    Qualification = item.Object.Qualification,
                    Skills = item.Object.Skills,
                    Experience = item.Object.Experience,
                    Expertise = item.Object.Expertise,
                    Location = item.Object.Location,
                    JobPreferences = item.Object.JobPreferences,
                    ProfilePictureUrl = item.Object.ProfilePictureUrl
                }).ToList();

                CandidatesListView.ItemsSource = allCandidates;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load candidates: " + ex.Message, "OK");
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue.ToLower();

            CandidatesListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
                ? allCandidates
                : allCandidates.Where(c => c.Skills.ToLower().Contains(searchText) ||
                                           c.Experience.ToLower().Contains(searchText) ||
                                           c.JobPreferences.ToLower().Contains(searchText)).ToList();
        }

        private async void OnCandidateSelected(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Candidate selectedCandidate)
            {
                await DisplayAlert("Candidate Selected", $"Name: {selectedCandidate.Name}\nSkills: {selectedCandidate.Skills}\nExperience: {selectedCandidate.Experience}\nPhone: {selectedCandidate.Phone}\nEmail: {selectedCandidate.Email}\nJobPreferences: {selectedCandidate.JobPreferences}\nQualification: {selectedCandidate.Qualification}", "OK");
            }
        }
    }
}


