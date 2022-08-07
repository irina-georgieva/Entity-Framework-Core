using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ExportDto
{
    [XmlType("Coach")]
    public class ExportCoachesDto
    {
        [XmlElement("CoachName")]
        public string CoachName { get; set; }

        [XmlArray("Footballers")]
        public ExportCoachFootballerDto[] Footballers { get; set; }

        [XmlAttribute("FootballersCount")]
        public int FootballersCount { get; set; }
    }
}
