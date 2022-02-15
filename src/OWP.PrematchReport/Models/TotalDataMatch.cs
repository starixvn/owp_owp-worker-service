using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWP.PrematchReport.Models
{
    public class TotalDataMatch
    {
        public int OWPLeagueId { get; set; }

        public int Total { get; set; }

        public bool IsMatchDay { get; set; }
    }
}