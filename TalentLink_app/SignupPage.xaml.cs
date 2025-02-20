using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using TalentLink_app.Services;

namespace TalentLink_app
{
    public partial class SignupPage : ContentPage
    {
        private FirebaseAuthService _authService = new FirebaseAuthService();

        public SignupPage()
        {
            InitializeComponent();
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;
            string role = RolePicker.SelectedItem?.ToString(); // Get selected role

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            try
            {
                string userId = await _authService.SignUpWithEmailPassword(email, password, role);
                if (!string.IsNullOrEmpty(userId))
                {
                    await DisplayAlert("Success", "Signup successful! Please verify your email.", "OK");
                    await Navigation.PopAsync(); // Navigate back to login
                }
                else
                {
                    await DisplayAlert("Error", "Signup failed", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }

    }
}
