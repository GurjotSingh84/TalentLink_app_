using System;
using Microsoft.Maui.Controls;
using TalentLink_app.Services;

namespace TalentLink_app
{
    public partial class ForgotPasswordPage : ContentPage
    {
        private FirebaseAuthService _authService;

        public ForgotPasswordPage()
        {
            InitializeComponent();
            _authService = new FirebaseAuthService();
        }

        private async void OnResetPasswordClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;

            if (string.IsNullOrEmpty(email))
            {
                await DisplayAlert("Error", "Please enter your email!", "OK");
                return;
            }

            try
            {
                await _authService.ResetPassword(email);
                await DisplayAlert("Success", "Password reset email sent! Check your inbox.", "OK");
                await Navigation.PopAsync(); // Navigate back to login page
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to reset password: {ex.Message}", "OK");
            }
        }
    }
}