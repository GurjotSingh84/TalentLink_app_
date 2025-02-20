using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink_app.Models
{
    public class JobApplication
    {
        public string ApplicationId { get; set; } = Guid.NewGuid().ToString();
        public string CandidateId { get; set; }
        public string JobId { get; set; }
        public string ResumeUrl { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Experience { get; set; }
        public string ExpectedPay { get; set; }
        public string Availability { get; set; }
        public string Qualifications { get; set; }
        public DateTime AppliedDate { get; set; } 
        public string Status { get; set; }

       
    }
}
