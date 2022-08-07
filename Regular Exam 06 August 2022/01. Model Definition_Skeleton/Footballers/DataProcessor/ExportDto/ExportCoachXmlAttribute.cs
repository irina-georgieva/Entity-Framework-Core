using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ExportDto
{
    [XmlType("Coach")]
    public class ExportCoachXmlAttribute
    {
        [XmlAttribute("FootballersCount")]
        public int FootballersCount { get; set; }
    }
}
