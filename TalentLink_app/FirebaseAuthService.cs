using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Maui.Storage;
 // ✅ This is required for FirebaseAuthProvider


namespace TalentLink_app.Services
{
    public class FirebaseAuthService
    {
        private const string ApiKey = "AIzaSyDhL7OpHJRWZ6uPaohUb_fnBbWpob4yavk";
        private const string DatabaseUrl = "https://aimadeinafrica-9e4ee-default-rtdb.firebaseio.com/";

        private FirebaseAuthProvider _authProvider;
        private FirebaseClient _firebaseClient;

        public FirebaseAuthService()
        {
            _authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            _firebaseClient = new FirebaseClient(DatabaseUrl);
        }

        // ✅ Sign Up Method with Role Selection and Email Verification
        public async Task<string> SignUpWithEmailPassword(string email, string password, string role)
        {
            try
            {
                var authResult = await _authProvider.CreateUserWithEmailAndPasswordAsync(email, password);
                string userId = authResult.User.LocalId; // ✅ CandidateID/RecruiterID

                if (!string.IsNullOrEmpty(userId))
                {
                    await SaveUserRole(userId, email, role);
                    Preferences.Set("UserID", userId); // ✅ Store UserID locally
                    return userId;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Signup failed: " + ex.Message);
            }
        }

        // ✅ Save User Role in Firebase Realtime Database
        private async Task SaveUserRole(string userId, string email, string role)
        {
            await _firebaseClient
                .Child("users")
                .Child(userId)
                .PutAsync(new { email = email, role = role.Trim() });
        }

        // ✅ Send Email Verification
        public async Task SendEmailVerification(string email, string password)
        {
            try
            {
                var authResult = await _authProvider.SignInWithEmailAndPasswordAsync(email, password);
                string idToken = authResult.FirebaseToken;  // ✅ Get Token

                await _authProvider.SendEmailVerificationAsync(idToken);  // ✅ Send Verification
                Console.WriteLine("Verification email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email verification failed: " + ex.Message);
                throw new Exception("Email verification failed: " + ex.Message);
            }
        }


        // ✅ Sign In Method
        public async Task<string> LoginWithEmailPassword(string email, string password)
        {
            try
            {
                var authResult = await _authProvider.SignInWithEmailAndPasswordAsync(email, password);
                string userId = authResult.User.LocalId;
                string token = authResult.FirebaseToken; // ✅ Store Firebase Token

                if (!string.IsNullOrEmpty(userId))
                {
                    Preferences.Set("UserID", userId);
                    Preferences.Set("UserToken", token); // ✅ Save token
                    return userId;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Login failed: " + ex.Message);
            }
        }


        // ✅ Fetch User Role from Firebase
        // ✅ Fetch user role from Firebase
        public async Task<string> GetUserRole()
        {
            var userId = await GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User ID not found");

            var token = Preferences.Get("UserToken", null);
            if (string.IsNullOrEmpty(token))
                throw new Exception("User not authenticated");

            var roleSnapshot = await _firebaseClient
                .Child("users")
                .Child(userId)
                .Child("role")
                .WithAuth(() => token)
                .OnceSingleAsync<string>();

            return roleSnapshot?.Trim();
        }



        public async Task<bool> PostJob(string recruiterId, string jobTitle, string jobDescription,
                                string jobRequirements, string jobLocation, string salary,
                                string companyName, string companyDetails,
                                string assessmentTitle, string assessmentDescription)
        {
            try
            {
                var jobData = new
                {
                    RecruiterId = recruiterId,
                    JobTitle = jobTitle,
                    JobDescription = jobDescription,
                    JobRequirements = jobRequirements,
                    JobLocation = jobLocation,
                    Salary = salary,
                    Company = new
                    {
                        Name = companyName,
                        Details = companyDetails
                    },
                    Assessment = new
                    {
                        Title = assessmentTitle,
                        Description = assessmentDescription
                    },
                    PostedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                await _firebaseClient
                    .Child("jobs")
                    .PostAsync(jobData);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Job posting failed: " + ex.Message);
            }
        }
        public async Task ResetPassword(string email)
        {
            try
            {
                await _authProvider.SendPasswordResetEmailAsync(email);
                Console.WriteLine($"Password reset email sent to: {email}");
            }
            catch (Exception ex)
            {
                throw new Exception("Password reset failed: " + ex.Message);
            }
        }

        // ✅ Get stored User ID from Preferences
        public async Task<string> GetUserId()
        {
            var userId = Preferences.Get("UserID", null);
            return !string.IsNullOrEmpty(userId) ? userId : null;
        }


        private async Task<string> GetToken()
        {
            var auth = await _authProvider.SignInWithEmailAndPasswordAsync("user@example.com", "password");
            return auth.FirebaseToken;
        }
        // ✅ Get user details (email & role) from Firebase
        public async Task<(string Email, string Role)> GetUserDetails(string userId)
        {
            try
            {
                var token = Preferences.Get("UserToken", null);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("User not authenticated");

                var userSnapshot = await _firebaseClient
                    .Child("users")
                    .Child(userId)
                    .WithAuth(() => token)
                    .OnceSingleAsync<dynamic>();

                if (userSnapshot != null)
                {
                    string email = userSnapshot.email;
                    string role = userSnapshot.role;
                    return (email, role);
                }

                return (null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching user details: {ex.Message}");
                return (null, null);
            }
        }





    }
}

