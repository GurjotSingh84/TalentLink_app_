using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink_app.Models
{
    public class Job
    {
        public string JobId { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string PayRate { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        // ✅ Recruiter and Candidate IDs
        public string RecruiterId { get; set; } = string.Empty;
        public string CandidateId { get; set; } = string.Empty;

        // ✅ Company Details
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyWebsite { get; set; } = string.Empty;

        // ✅ Contact Information
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;

        // ✅ Timestamp
        public string PostedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    }


}



