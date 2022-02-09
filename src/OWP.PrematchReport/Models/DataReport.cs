using OWP.Core.Attributes;
using System;
using System.Xml.Serialization;

namespace OWP.PrematchReport.Models
{
    [Serializable]
    [XmlName("Row")]
    public record DataReport
    {
        public int ReportId { get; set; }

        [XmlIgnore]
        public int UserId { get; set; }

        [XmlIgnore]
        public DateTime EventDate { get; set; }

        public int GroupId { get; set; }

        public int Amount { get; set; }

        public int BetType { get; set; }
    }
}