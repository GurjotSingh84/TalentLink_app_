using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Storage;
using Firebase.Database;
using Firebase.Database.Query;
using System.Text.RegularExpressions;
using TalentLink_app.Models;
using TalentLink_app.Services;
using Microsoft.Maui.Storage; // Ensure FilePicker works properly
using Microsoft.Maui.Controls; // Ensure ContentPage is recognized

namespace TalentLink_app
{
    public partial class ApplyPage : ContentPage
    {
        private string _resumeFilePath = "";
        private readonly FirebaseClient _firebase = new FirebaseClient("https://aimadeinafrica-9e4ee-default-rtdb.firebaseio.com/");
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();

        private string _jobId;
        private string _candidateId;

        public ApplyPage(string jobId)
        {
            InitializeComponent();
            _jobId = jobId;
            LoadUserData();
        }

        private async void LoadUserData()
        {
            try
            {
                _candidateId = await _authService.GetUserId(); // ✅ Get UID as Candidate ID

                if (string.IsNullOrEmpty(_candidateId))
                {
                    await DisplayAlert("Error", "Authentication required.", "OK");
                    return;
                }

                // ✅ Fix: Handle tuple properly
                var user = await _authService.GetUserDetails(_candidateId);

                if (!string.IsNullOrEmpty(user.Email)) // ✅ Ensure Email is not empty
                {
                    NameEntry.Text = user.Email.Split('@')[0]; // Extract name from Email (or modify as needed)
                    EmailEntry.Text = user.Email;
                    PhoneEntry.Text = ""; // 🔹 Fetch from user profile if available
                }
                else
                {
                    await DisplayAlert("Error", "Failed to load user data.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load user data: {ex.Message}", "OK");
            }
        }


        private async void OnResumeUploadClicked(object sender, EventArgs e)
        {
            try
            {
                var fileResult = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select your Resume",
                    FileTypes = FilePickerFileType.Pdf
                });

                if (fileResult != null)
                {
                    _resumeFilePath = fileResult.FullPath;
                    ResumeStatusLabel.Text = "Uploaded: " + fileResult.FileName;

                    // Extract text from resume (Optional feature)
                    string extractedText = await ExtractTextFromResume(_resumeFilePath);
                    AutofillDetails(extractedText);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to upload resume: " + ex.Message, "OK");
            }
        }

        private async Task<string> ExtractTextFromResume(string filePath)
        {
            // Placeholder function – Implement PDF parsing here
            return "John Doe\njohndoe@example.com\n123-456-7890\nExperience: 5 years\nQualifications: BSc Computer Science\nExpected Pay: $50,000/year\nAvailability: Immediate";
        }

        private void AutofillDetails(string text)
        {
            NameEntry.Text = ExtractValue(text, @"^([A-Za-z\s]+)");
            EmailEntry.Text = ExtractValue(text, @"\S+@\S+\.\S+");
            PhoneEntry.Text = ExtractValue(text, @"\d{3}[-.]\d{3}[-.]\d{4}");
            ExperienceEntry.Text = ExtractValue(text, @"Experience:\s*(.+)");
            ExpectedPayEntry.Text = ExtractValue(text, @"Expected Pay:\s*(.+)");
            AvailabilityEntry.Text = ExtractValue(text, @"Availability:\s*(.+)");
            QualificationsEntry.Text = ExtractValue(text, @"Qualifications:\s*(.+)");
        }

        private string ExtractValue(string text, string pattern)
        {
            var match = Regex.Match(text, pattern);
            return match.Success ? match.Groups[1].Value.Trim() : "";
        }

        private async void OnSubmitApplicationClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PhoneEntry.Text) ||
                string.IsNullOrWhiteSpace(_resumeFilePath))
            {
                await DisplayAlert("Error", "Please fill all details and upload resume.", "OK");
                return;
            }

            string applicationId = Guid.NewGuid().ToString();

            if (string.IsNullOrEmpty(_candidateId))
            {
                await DisplayAlert("Error", "User not authenticated.", "OK");
                return;
            }

            var applicationData = new JobApplication
            {
                ApplicationId = applicationId,
                CandidateId = _candidateId,
                JobId = _jobId,
                Name = NameEntry.Text,
                Email = EmailEntry.Text,
                Phone = PhoneEntry.Text,
                ResumeUrl = _resumeFilePath,
                Experience = ExperienceEntry.Text,
                ExpectedPay = ExpectedPayEntry.Text,
                Availability = AvailabilityEntry.Text,
                Qualifications = QualificationsEntry.Text,
                Status = "Pending"
            };

            var candidateData = new Candidate
            {
                CandidateId = _candidateId,
                JobId = _jobId,
                Name = NameEntry.Text,
                Email = EmailEntry.Text,
                Phone = PhoneEntry.Text,
                ResumeUrl = _resumeFilePath,
                Experience = ExperienceEntry.Text,
                ExpectedPay = ExpectedPayEntry.Text,
                Availability = AvailabilityEntry.Text,
                Qualifications = QualificationsEntry.Text,
                Status = "Pending"
            };

            try
            {
                await _firebase.Child("JobApplications").Child(applicationId).PutAsync(applicationData);
                await _firebase.Child("Candidates").Child(_candidateId).PutAsync(candidateData); // ✅ Store candidate data

                await DisplayAlert("Success", "Your application has been submitted!", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Application submission failed: {ex.Message}", "OK");
            }
        }

    }
}







