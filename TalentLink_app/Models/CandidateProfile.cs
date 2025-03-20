using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink_app.Models
{
    public class CandidateProfile
    {
        public string CandidateId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Skills { get; set; }
        public int Experience { get; set; }
        public string Location { get; set; }
    }

}
