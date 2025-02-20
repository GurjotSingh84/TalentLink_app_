using Microsoft.Maui.Controls;
using TalentLink_app.Models;

namespace TalentLink_app
{
    public partial class CandidateDetailsPage : ContentPage
    {
        public CandidateDetailsPage(Candidate candidate)
        {
            InitializeComponent();

            if (candidate != null)
            {
                CandidateIdLabel.Text = $"Candidate ID: {candidate.CandidateId}"; // ✅ Show UID
                NameLabel.Text = $"Name: {candidate.Name}";
                EmailLabel.Text = $"Email: {candidate.Email}";
                PhoneLabel.Text = $"Phone: {candidate.Phone}";
                ExperienceLabel.Text = $"Experience: {candidate.Experience}";
                ExpectedPayLabel.Text = $"Expected Pay: {candidate.ExpectedPay}";
                AvailabilityLabel.Text = $"Availability: {candidate.Availability}";
                QualificationsLabel.Text = $"Qualifications: {candidate.Qualifications}";
                StatusLabel.Text = $"Status: {candidate.Status}";
            }
        }
    }
}


