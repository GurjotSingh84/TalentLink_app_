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
       
        public string RecruiterId { get; set; } // ✅ Recruiter's ID
        public string CandidateId { get; set; } // ✅ Candidate's ID (if applied)
       
        
        
       
    
        public string PostedAt { get; set; }

    }

   
}



