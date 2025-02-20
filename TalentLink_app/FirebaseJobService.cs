using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentLink_app.Models;
using TalentLink_app.Services;


namespace TalentLink_app.Services
{
    public class FirebaseJobService
    {
        private readonly FirebaseClient _firebase = new FirebaseClient("https://aimadeinafrica-9e4ee-default-rtdb.firebaseio.com/");

        public async Task<bool> PostJob(Job job)
        {
            try
            {
                await _firebase.Child("Jobs").Child(job.JobId).PutAsync(job);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Job>> GetJobs()
        {
            return (await _firebase.Child("Jobs").OnceAsync<Job>())
                .Select(item => new Job
                {
                    JobId = item.Key,
                    JobTitle = item.Object.JobTitle,
                    JobDescription = item.Object.JobDescription,
                    PayRate = item.Object.PayRate,
                    Location = item.Object.Location
                }).ToList();
        }

        public async Task<bool> EditJob(Job job)
        {
            try
            {
                await _firebase.Child("Jobs").Child(job.JobId).PutAsync(job);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteJob(string jobId)
        {
            try
            {
                await _firebase.Child("Jobs").Child(jobId).DeleteAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }



        public async Task<List<Job>> GetAllJobs()
        {
            return (await _firebase
                .Child("Jobs")  // ✅ Correcting the case
                .OnceAsync<Job>())
                .Select(item => new Job
                {
                    JobId = item.Key,  // ✅ Ensure JobId is set correctly
                    JobTitle = item.Object.JobTitle,
                    JobDescription = item.Object.JobDescription,
                    Location = item.Object.Location,
                    PayRate = item.Object.PayRate
                }).ToList();
        }

        public async Task<bool> ApplyForJob(JobApplication jobApplication)
        {
            try
            {
                await _firebase
                    .Child("JobApplications")
                    .Child(jobApplication.ApplicationId)
                    .PutAsync(jobApplication);

                // 🔹 Store in "appliedJobs" as well (if needed)
                await _firebase
                    .Child("appliedJobs")
                    .Child(jobApplication.CandidateId)
                    .Child(jobApplication.ApplicationId)
                    .PutAsync(new AppliedJob
                    {
                        JobId = jobApplication.JobId,
                        CandidateId = jobApplication.CandidateId,
                       
                    });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error applying for job: {ex.Message}");
                return false;
            }
        }





        public async Task<bool> WithdrawApplication(string jobId, string candidateId)
        {
            var applications = await _firebase.Child("JobApplications").OnceAsync<JobApplication>(); // ✅ Use Correct Model Name


            var applicationToRemove = applications
                .FirstOrDefault(a => a.Object.JobId == jobId && a.Object.CandidateId == candidateId);

            if (applicationToRemove != null)
            {
                await _firebase.Child("JobApplications").Child(applicationToRemove.Key).DeleteAsync();
                return true;
            }
            return false;
        }
        public async Task<List<Job>> GetAppliedJobs(string candidateId)
        {
            var applications = await _firebase.Child("JobApplications").OnceAsync<JobApplication>();

            // ✅ Correct variable usage - filter applications for the given candidate
            var candidateApplications = applications
                .Where(a => a.Object.CandidateId == candidateId)
                .Select(a => a.Object.JobId)
                .ToList();

            if (!candidateApplications.Any()) return new List<Job>(); // Return empty if no applications

            var allJobs = await _firebase.Child("Jobs").OnceAsync<Job>();

            // ✅ Fetch only jobs that the candidate applied for
            return allJobs
                .Where(j => candidateApplications.Contains(j.Object.JobId)) // Match JobIds
                .Select(j => j.Object)
                .ToList();
        }



        public async Task<List<Candidate>> GetCandidatesForJob(string jobId)
        {
            List<Candidate> candidatesList = new List<Candidate>();

            try
            {
                var candidatesSnapshot = await _firebase
                    .Child("Applications")
                    .OrderBy("JobID")
                    .EqualTo(jobId)
                    .OnceAsync<Candidate>();

                foreach (var candidate in candidatesSnapshot)
                {
                    candidatesList.Add(candidate.Object);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Firebase Error: {ex.Message}");
            }

            return candidatesList;
        }



        public async Task<bool> PostJob(string recruiterId, string jobTitle, string jobDescription,
                                      string jobLocation, string salary)
        {
            try
            {
                string jobId = Guid.NewGuid().ToString(); // ✅ Generate Unique Job ID

                var jobData = new Job
                {
                    JobId = jobId,
                    RecruiterId = recruiterId,
                    JobTitle = jobTitle,
                    JobDescription = jobDescription,
                    Location = jobLocation,
                    PayRate = salary,
                    PostedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                await _firebase
                    .Child("jobs")
                    .Child(jobId)
                    .PutAsync(jobData);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Job posting failed: " + ex.Message);
            }






        }
        // ✅ Save a job for a candidate
        public async Task<bool> SaveJob(string candidateId, string jobId)
        {
            try
            {
                await _firebase
                    .Child("SavedJobs")
                    .Child(candidateId)
                    .Child(jobId)
                    .PutAsync(true); // Store as a simple boolean
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ✅ Get all saved jobs for a candidate
        public async Task<List<Job>> GetSavedJobs(string candidateId)
        {
            var savedJobsRef = _firebase.Child($"SavedJobs/{candidateId}");
            var savedJobsSnapshot = await savedJobsRef.OnceAsync<object>();

            List<Job> savedJobsList = new List<Job>();

            foreach (var jobEntry in savedJobsSnapshot)
            {
                string jobId = jobEntry.Key; // Get Job ID

                // 🔍 Fetch full job details using Job ID
                var jobDetailsRef = _firebase.Child($"Jobs/{jobId}");
                var jobDetails = await jobDetailsRef.OnceSingleAsync<Job>();

                if (jobDetails != null)
                {
                    savedJobsList.Add(jobDetails);
                }
            }

            return savedJobsList;
        }


        // ✅ Unsave a job for a candidate
        public async Task<bool> UnsaveJob(string candidateId, string jobId)
        {
            try
            {
                await _firebase
                    .Child("SavedJobs")
                    .Child(candidateId)
                    .Child(jobId)
                    .DeleteAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<Job>> GetJobsByRecruiter(string recruiterId)
        {
            try
            {
                var allJobs = await _firebase.Child("Jobs").OnceAsync<Job>();

                // 🔹 Filter jobs posted by this recruiter
                return allJobs
                    .Where(j => j.Object.RecruiterId == recruiterId) // ✅ Filter by RecruiterId
                    .Select(j => new Job
                    {
                        JobId = j.Key,  // ✅ Ensure JobId is set correctly
                        RecruiterId = j.Object.RecruiterId,
                        JobTitle = j.Object.JobTitle,
                        JobDescription = j.Object.JobDescription,
                        Location = j.Object.Location,
                        PayRate = j.Object.PayRate,
                        PostedAt = j.Object.PostedAt
                    }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching jobs for recruiter {recruiterId}: {ex.Message}");
                return new List<Job>();
            }
        }
       
        public async Task<Job> GetJobById(string jobId)
        {
            try
            {
                var jobSnapshot = await _firebase
                    .Child("jobs")
                    .Child(jobId)
                    .OnceSingleAsync<Job>();

                return jobSnapshot;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching job by ID: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Job>> GetAppliedJobsByCandidate(string candidateId)
        {
            var applications = await _firebase.Child("JobApplications").OnceAsync<JobApplication>();

            // ✅ Get all Job IDs where the candidate applied
            var candidateApplications = applications
                .Where(a => a.Object.CandidateId == candidateId)
                .Select(a => a.Object.JobId)
                .ToList();

            if (!candidateApplications.Any()) return new List<Job>(); // Return empty if no applications

            var allJobs = await _firebase.Child("Jobs").OnceAsync<Job>();

            // ✅ Fetch only jobs that the candidate applied for
            return allJobs
                .Where(j => candidateApplications.Contains(j.Object.JobId)) // Match JobIds
                .Select(j => j.Object)
                .ToList();
        }





        public async Task<List<Candidate>> GetCandidatesForRecruiter(string recruiterId)
        {
            List<Candidate> candidatesList = new List<Candidate>();

            try
            {
                var jobsSnapshot = await _firebase
                    .Child("Jobs")
                    .OrderBy("RecruiterID")
                    .EqualTo(recruiterId)
                    .OnceAsync<Job>();

                foreach (var job in jobsSnapshot)
                {
                    var candidatesSnapshot = await _firebase
                        .Child("Applications")
                        .OrderBy("JobID")
                        .EqualTo(job.Object.JobId)
                        .OnceAsync<Candidate>();

                    foreach (var candidate in candidatesSnapshot)
                    {
                        candidatesList.Add(candidate.Object);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Firebase Error: {ex.Message}");
            }

            return candidatesList;
        }








    }
}




