using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink_app.Models
{
    public class Notification
    {
        public string CandidateId { get; set; }
        public string RecruiterId { get; set; }
        public string JobId { get; set; }
        public string Status { get; set; } // Approved / Rejected
        public string Message { get; set; }
        public string Timestamp { get; set; }
    }
}
