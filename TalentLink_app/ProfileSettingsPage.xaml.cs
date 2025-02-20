using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Storage;
using Microsoft.Maui.Storage;
using TalentLink_app.Services;
using Firebase.Database.Query;

namespace TalentLink_app
{
    public partial class ProfileSettingsPage : ContentPage
    {
        private FirebaseAuthService _authService = new FirebaseAuthService();
        private FirebaseClient firebaseClient = new FirebaseClient("https://aimadeinafrica-9e4ee-default-rtdb.firebaseio.com/");
        private FirebaseStorage firebaseStorage = new FirebaseStorage("aimadeinafrica-9e4ee.appspot.com");

        private string profilePictureUrl = "";
        private string userId;

        public ProfileSettingsPage()
        {
            InitializeComponent();
            LoadProfileData();
        }

        private async void LoadProfileData()
        {
            userId = await _authService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                await DisplayAlert("Error", "User not authenticated", "OK");
                return;
            }

            var user = await firebaseClient.Child("users").Child(userId).OnceSingleAsync<UserProfile>();

            if (user != null)
            {
                NameEntry.Text = user.Name;
                EmailEntry.Text = user.Email;
                PhoneEntry.Text = user.Phone;
                QualificationEntry.Text = user.Qualification;
                SkillsEditor.Text = user.Skills;
                ExperienceEntry.Text = user.Experience;
                ExpertiseEditor.Text = user.Expertise;
                LocationEntry.Text = user.Location;
                JobPreferencesEditor.Text = user.JobPreferences;

                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    ProfileImage.Source = user.ProfilePictureUrl;
                    profilePictureUrl = user.ProfilePictureUrl;
                }
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(userId))
            {
                await DisplayAlert("Error", "User ID not found", "OK");
                return;
            }

            var profile = new UserProfile
            {
                Name = NameEntry.Text,
                Email = EmailEntry.Text,
                Phone = PhoneEntry.Text,
                Qualification = QualificationEntry.Text,
                Skills = SkillsEditor.Text,
                Experience = ExperienceEntry.Text,
                Expertise = ExpertiseEditor.Text,
                Location = LocationEntry.Text,
                JobPreferences = JobPreferencesEditor.Text,
                ProfilePictureUrl = profilePictureUrl
            };

            await firebaseClient.Child("users").Child(userId).PutAsync(profile);
            await DisplayAlert("Success", "Profile updated successfully!", "OK");
        }

        private async void OnUploadProfilePictureClicked(object sender, EventArgs e)
        {
            var result = await MediaPicker.PickPhotoAsync();
            if (result != null)
            {
                var fileStream = await result.OpenReadAsync();
                var storagePath = $"profile_pictures/{userId}/{Path.GetFileName(result.FullPath)}";

                var uploadTask = await firebaseStorage.Child(storagePath).PutAsync(fileStream);
                profilePictureUrl = await firebaseStorage.Child(storagePath).GetDownloadUrlAsync();

                ProfileImage.Source = profilePictureUrl;
            }
        }
    }

    public class UserProfile
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Qualification { get; set; }
        public string Skills { get; set; }
        public string Experience { get; set; }
        public string Expertise { get; set; }
        public string Location { get; set; }
        public string JobPreferences { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
