using Footballers.Common;
using Footballers.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ImportDto
{
    [XmlType("Footballer")]
    public class ImportCoachFootballersDto
    {
        [Required]
        [MinLength(ValidationConstants.FootballerNameMinLength)]
        [MaxLength(ValidationConstants.FootballerNameMaxLength)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        [XmlElement("ContractStartDate")]
        public string ContractStartDate { get; set; }

        [Required]
        [XmlElement("ContractEndDate")]
        public string ContractEndDate { get; set; }

        [Required]
        [XmlElement("BestSkillType")]
        public string BestSkillType { get; set; }

        [Required]
        [XmlElement("PositionType")]
        public string PositionType { get; set; }
    }
}
