using System;
using Microsoft.Maui.Controls;
using TalentLink_app.Services;

namespace TalentLink_app
{
    public partial class LoginPage : ContentPage
    {
        private FirebaseAuthService _authService;

        public LoginPage()
        {
            InitializeComponent();
            _authService = new FirebaseAuthService();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter email and password!", "OK");
                return;
            }

            try
            {
                await _authService.LoginWithEmailPassword(email, password);
                string role = await _authService.GetUserRole(); // ✅ Removed `userId` argument

                Console.WriteLine($"DEBUG: Retrieved Role -> {role}"); // Debugging

                if (role.Equals("Recruiter", StringComparison.OrdinalIgnoreCase))
                {
                    await Navigation.PushAsync(new RecruiterHomePage());
                }
                else if (role.Equals("Candidate", StringComparison.OrdinalIgnoreCase))
                {
                    await Navigation.PushAsync(new CandidateHomePage());
                }
                else
                {
                    await DisplayAlert("Error", $"Invalid role assigned! Retrieved: {role}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Login Failed", ex.Message, "OK");
            }
        }


        private async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignupPage());
        }

        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ForgotPasswordPage());
        }

    }
}
